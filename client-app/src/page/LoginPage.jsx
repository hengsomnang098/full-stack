import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Checkbox, Form, Input, Typography, message } from "antd";
import { Link, useNavigate } from "react-router-dom";
import { request } from "../config/request";
import { setUser } from "../config/helper";
import { brand } from "../theme";

const LoginPage = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();

  const onFinish = async (values) => {
    const username = values.username?.trim();
    const password = values.password;
    if (!username || !password) {
      message.warning("Please fill in username and password.");
      return;
    }

    const res = await request("employee/login", "post", {
      Username: username,
      Password: password,
    });

    if (!res) return;

    if (res.error) {
      message.error(res.message || "Login failed");
      return;
    }

    setUser(res.user);
    navigate("/");
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-card__brand">
          <div className="auth-card__logo">N</div>
          <h1 className="auth-card__title">{brand.name}</h1>
          <p className="auth-card__subtitle">Sign in to manage sales and inventory</p>
        </div>

        <Form
          form={form}
          name="login"
          layout="vertical"
          requiredMark={false}
          initialValues={{ remember: true }}
          onFinish={onFinish}
        >
          <Form.Item
            name="username"
            label="Username"
            rules={[{ required: true, message: "Please input your username" }]}
          >
            <Input
              size="large"
              prefix={<UserOutlined />}
              placeholder="Phone / username"
              autoComplete="username"
            />
          </Form.Item>

          <Form.Item
            name="password"
            label="Password"
            rules={[{ required: true, message: "Please input your password" }]}
          >
            <Input.Password
              size="large"
              prefix={<LockOutlined />}
              placeholder="Password"
              autoComplete="current-password"
            />
          </Form.Item>

          <Form.Item>
            <div className="auth-card__meta">
              <Form.Item name="remember" valuePropName="checked" noStyle>
                <Checkbox>Remember me</Checkbox>
              </Form.Item>
              <Typography.Text type="secondary">Forgot password?</Typography.Text>
            </div>
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" size="large">
              Log in
            </Button>
          </Form.Item>

          <Typography.Paragraph type="secondary" style={{ textAlign: "center", marginBottom: 0 }}>
            Need an account? <Link to="/register">Register</Link>
          </Typography.Paragraph>
        </Form>
      </div>
    </div>
  );
};

export default LoginPage;
