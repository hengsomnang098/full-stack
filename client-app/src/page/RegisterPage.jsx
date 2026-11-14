import { Typography } from "antd";
import { Link } from "react-router-dom";
import { brand } from "../theme";

const RegisterPage = () => {
  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-card__brand">
          <div className="auth-card__logo">N</div>
          <h1 className="auth-card__title">{brand.name}</h1>
          <p className="auth-card__subtitle">Registration is managed by an administrator.</p>
        </div>
        <Typography.Paragraph style={{ textAlign: "center", marginBottom: 24 }}>
          Ask your admin to create an employee account, then sign in with your phone number.
        </Typography.Paragraph>
        <div style={{ textAlign: "center" }}>
          <Link to="/login">Back to login</Link>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
