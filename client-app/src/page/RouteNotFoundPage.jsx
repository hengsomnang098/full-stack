import { Button, Result } from "antd";
import { useNavigate } from "react-router-dom";

const RouteNotFoundPage = () => {
  const navigate = useNavigate();
  return (
    <Result
      status="404"
      title="Page not found"
      subTitle="That route does not exist in NIT Company."
      extra={
        <Button type="primary" onClick={() => navigate("/")}>
          Back to dashboard
        </Button>
      }
    />
  );
};

export default RouteNotFoundPage;
