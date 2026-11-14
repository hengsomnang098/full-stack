import {
  TeamOutlined,
  ShopOutlined,
  ProductOutlined,
  FileTextOutlined,
  AppstoreOutlined,
  UserOutlined,
  CreditCardOutlined,
  SafetyCertificateOutlined,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import MainPage from "../components/page/MainPage";
import { getUser } from "../config/helper";
import { brand } from "../theme";

const tiles = [
  {
    path: "/pos",
    label: "POS",
    desc: "Search products and check out orders",
    icon: <ShopOutlined />,
  },
  {
    path: "/product",
    label: "Products",
    desc: "Manage catalog, price, and images",
    icon: <ProductOutlined />,
  },
  {
    path: "/customer",
    label: "Customers",
    desc: "Customer profiles and contacts",
    icon: <TeamOutlined />,
  },
  {
    path: "/invoice",
    label: "Invoices",
    desc: "Review sales history",
    icon: <FileTextOutlined />,
  },
  {
    path: "/category",
    label: "Categories",
    desc: "Organize product groups",
    icon: <AppstoreOutlined />,
  },
  {
    path: "/employee",
    label: "Employees",
    desc: "Staff accounts and roles",
    icon: <UserOutlined />,
  },
  {
    path: "/payment-method",
    label: "Payments",
    desc: "Configure payment methods",
    icon: <CreditCardOutlined />,
  },
  {
    path: "/order-status",
    label: "Order status",
    desc: "Workflow statuses for orders",
    icon: <SafetyCertificateOutlined />,
  },
];

const HomePage = () => {
  const navigate = useNavigate();
  const user = getUser();
  const first = user?.Firstname || user?.FirstName || "there";

  return (
    <MainPage>
      <div className="dash-hero">
        <h1>Welcome back, {first}</h1>
        <p>
          Quick access to the tools you use most in {brand.name}. Everything
          scales cleanly from phone to desktop.
        </p>
      </div>

      <div className="dash-grid">
        {tiles.map((tile) => (
          <button
            key={tile.path}
            type="button"
            className="dash-tile"
            onClick={() => navigate(tile.path)}
          >
            <span className="dash-tile__icon">{tile.icon}</span>
            <span className="dash-tile__label">{tile.label}</span>
            <span className="dash-tile__desc">{tile.desc}</span>
          </button>
        ))}
      </div>
    </MainPage>
  );
};

export default HomePage;
