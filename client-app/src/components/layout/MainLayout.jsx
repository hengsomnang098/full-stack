import { useEffect, useMemo, useState } from "react";
import {
  AppstoreOutlined,
  TeamOutlined,
  UserOutlined,
  ProductOutlined,
  ShopOutlined,
  LogoutOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  FileTextOutlined,
  CreditCardOutlined,
  SafetyCertificateOutlined,
} from "@ant-design/icons";
import { Avatar, Button, Drawer, Grid, Layout, Menu, Typography } from "antd";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { getUser, isLogin, logout } from "../../config/helper";
import { brand } from "../../theme";

const { Header, Content, Sider, Footer } = Layout;
const { useBreakpoint } = Grid;

function getItem(label, key, icon, children) {
  return { key, icon, children, label };
}

const items = [
  getItem("Dashboard", "/", <AppstoreOutlined />),
  getItem("Customer", "/customer", <TeamOutlined />),
  getItem("Employee", "/employee", <TeamOutlined />),
  getItem("POS", "/pos", <ShopOutlined />),
  getItem("Invoice", "/invoice", <FileTextOutlined />),
  getItem("Product", "product-group", <ProductOutlined />, [
    getItem("Product", "/product"),
    getItem("Product stock", "/product-stock"),
    getItem("Category", "/category"),
  ]),
  getItem("System", "system", <UserOutlined />, [
    getItem("Order status", "/order-status", <SafetyCertificateOutlined />),
    getItem("Payment method", "/payment-method", <CreditCardOutlined />),
    getItem("Role", "/role"),
  ]),
];

const pathTitles = {
  "/": "Dashboard",
  "/customer": "Customers",
  "/employee": "Employees",
  "/pos": "Point of Sale",
  "/invoice": "Invoices",
  "/product": "Products",
  "/product-stock": "Product stock",
  "/category": "Categories",
  "/order-status": "Order status",
  "/payment-method": "Payment methods",
  "/role": "Roles",
};

const BrandBlock = ({ compact = false }) => (
  <div className="app-sider-brand">
    <div className="app-sider-brand__logo">N</div>
    {!compact && (
      <div className="app-sider-brand__text">
        <div className="app-sider-brand__name">{brand.name}</div>
        <div className="app-sider-brand__tag">{brand.tagline}</div>
      </div>
    )}
  </div>
);

const MainLayout = () => {
  const user = getUser();
  const navigate = useNavigate();
  const location = useLocation();
  const screens = useBreakpoint();
  const isMobile = !screens.lg;

  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);

  useEffect(() => {
    if (!isLogin()) {
      navigate("/login");
    }
  }, [navigate]);

  useEffect(() => {
    setMobileOpen(false);
  }, [location.pathname]);

  const selectedKeys = useMemo(() => {
    const path = location.pathname === "" ? "/" : location.pathname;
    return [path.startsWith("/") ? path : `/${path}`];
  }, [location.pathname]);

  const openKeys = useMemo(() => {
    if (["/product", "/product-stock", "/category"].includes(location.pathname)) {
      return ["product-group"];
    }
    if (["/order-status", "/payment-method", "/role"].includes(location.pathname)) {
      return ["system"];
    }
    return [];
  }, [location.pathname]);

  const pageTitle = pathTitles[location.pathname] || "NIT Company";
  const displayName = `${user?.Firstname || user?.FirstName || ""} ${
    user?.Lastname || user?.LastName || ""
  }`.trim();

  const onClickMenu = ({ key }) => {
    if (key === "logout") {
      logout();
      return;
    }
    if (key === "product-group" || key === "system") return;
    navigate(key);
  };

  const menu = (
    <Menu
      theme="dark"
      mode="inline"
      selectedKeys={selectedKeys}
      defaultOpenKeys={openKeys}
      items={[
        ...items,
        { type: "divider" },
        getItem("Logout", "logout", <LogoutOutlined />),
      ]}
      onClick={onClickMenu}
    />
  );

  if (!user) {
    return null;
  }

  return (
    <Layout className="app-layout">
      {!isMobile && (
        <Sider
          className="app-sider"
          width={250}
          collapsible
          collapsed={collapsed}
          onCollapse={setCollapsed}
          breakpoint="lg"
        >
          <BrandBlock compact={collapsed} />
          {menu}
        </Sider>
      )}

      {isMobile && (
        <Drawer
          placement="left"
          width={280}
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          styles={{ body: { padding: 0, background: "#0B3D3A" } }}
          closable={false}
        >
          <BrandBlock />
          {menu}
        </Drawer>
      )}

      <Layout>
        <Header className="app-header">
          <div className="app-header__left">
            <Button
              type="text"
              aria-label={isMobile ? "Open menu" : "Toggle sidebar"}
              icon={
                isMobile || collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />
              }
              onClick={() => {
                if (isMobile) setMobileOpen(true);
                else setCollapsed((v) => !v);
              }}
            />
            <Typography.Title level={4} className="app-header__title">
              {pageTitle}
            </Typography.Title>
          </div>

          <div className="app-header__right">
            <div className="app-header__user">
              <Typography.Text className="app-header__user-name">
                {displayName || "User"}
              </Typography.Text>
              <Avatar style={{ backgroundColor: brand.primary }}>
                {(displayName || "U").charAt(0).toUpperCase()}
              </Avatar>
            </div>
          </div>
        </Header>

        <Content className="app-content">
          <div className="app-content__inner page-shell">
            <Outlet />
          </div>
        </Content>

        <Footer className="app-footer">
          {brand.name} ©{new Date().getFullYear()}
        </Footer>
      </Layout>
    </Layout>
  );
};

export default MainLayout;
