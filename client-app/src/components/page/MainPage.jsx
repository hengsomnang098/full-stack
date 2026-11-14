/* eslint-disable react/prop-types */
import { Card, Spin } from "antd";

const MainPage = ({ children, loading = false }) => {
  return (
    <Spin spinning={loading}>
      <Card className="app-page-card" bordered={false}>
        <div className="table-scroll">{children}</div>
      </Card>
    </Spin>
  );
};

export default MainPage;
