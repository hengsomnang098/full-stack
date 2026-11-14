import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { ConfigProvider, App as AntApp } from "antd";
import Homepage from "./page/Homepage";
import LoginPage from "./page/LoginPage";
import RegisterPage from "./page/RegisterPage";
import RouteNotFoundPage from "./page/RouteNotFoundPage";
import CategoryPage from "./page/CategoryPage";
import MainLayout from "./components/layout/MainLayout";
import OrderStatusPage from "./page/OrderStatusPage";
import PaymentMethodPage from "./page/PaymentMethodPage";
import RolePage from "./page/RolePage";
import EmployeePage from "./page/EmployeePage";
import CustomerPage from "./page/CustomerPage";
import ProductPage from "./page/ProductPage";
import POSPage from "./page/POSPage";
import InvoicePage from "./page/InvoicePage";
import { appTheme } from "./theme";

function App() {
  return (
    <ConfigProvider theme={appTheme}>
      <AntApp>
        <BrowserRouter>
          <Routes>
            <Route element={<MainLayout />}>
              <Route path="/" element={<Homepage />} />
              <Route path="/customer" element={<CustomerPage />} />
              <Route path="/pos" element={<POSPage />} />
              <Route path="/invoice" element={<InvoicePage />} />
              <Route path="/employee" element={<EmployeePage />} />
              <Route path="/product" element={<ProductPage />} />
              <Route path="/product-stock" element={<h1>product stock</h1>} />
              <Route path="/category" element={<CategoryPage />} />
              <Route path="/order-status" element={<OrderStatusPage />} />
              <Route path="/payment-method" element={<PaymentMethodPage />} />
              <Route path="/role" element={<RolePage />} />
              <Route path="*" element={<RouteNotFoundPage />} />
            </Route>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="*" element={<LoginPage />} />
          </Routes>
        </BrowserRouter>
      </AntApp>
    </ConfigProvider>
  );
}

export default App;
