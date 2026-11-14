const order_payment_method_controller = require("../controller/order_payment_method.controller");
const { CheckToken } = require("../controller/employee.controller");

const order_payment_method = (app) => {
  app.get(
    `/api/order_payment_method/list`,
    CheckToken(),
    order_payment_method_controller.getList
  );
  app.get(
    `/api/order_payment_method/getone`,
    CheckToken(),
    order_payment_method_controller.getById
  );
  app.post(
    `/api/order_payment_method/create`,
    CheckToken(),
    order_payment_method_controller.create
  );
  app.put(
    `/api/order_payment_method/update`,
    CheckToken(),
    order_payment_method_controller.update
  );
  app.delete(
    `/api/order_payment_method/delete`,
    CheckToken(),
    order_payment_method_controller.remove
  );
};

module.exports = order_payment_method;
