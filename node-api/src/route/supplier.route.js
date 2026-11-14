const supplier_controller = require("../controller/supplier.controller");
const { CheckToken } = require("../controller/employee.controller");

const supplier = (app) => {
  app.get("/api/supplier/getList", CheckToken(), supplier_controller.getList);
  app.get("/api/supplier/getById", CheckToken(), supplier_controller.getById);
  app.post("/api/supplier/create", CheckToken(), supplier_controller.create);
  app.put("/api/supplier/update", CheckToken(), supplier_controller.update);
  app.delete("/api/supplier/delete", CheckToken(), supplier_controller.remove);
};

module.exports = supplier;
