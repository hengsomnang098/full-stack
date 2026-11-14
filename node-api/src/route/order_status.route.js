const order_status_controller = require("../controller/order_status.controller");
const { CheckToken } = require("../controller/employee.controller");

const order_status = (app) => {
  app.get(`/api/order_status/list`, CheckToken(), order_status_controller.getList);
  app.get(`/api/order_status/getone`, CheckToken(), order_status_controller.getById);
  app.post(`/api/order_status/create`, CheckToken(), order_status_controller.create);
  app.put(`/api/order_status/update`, CheckToken(), order_status_controller.update);
  app.delete(`/api/order_status/delete`, CheckToken(), order_status_controller.remove);
};

module.exports = order_status;
