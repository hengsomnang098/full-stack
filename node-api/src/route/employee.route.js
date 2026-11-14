const controller = require("../controller/employee.controller");
const { CheckToken } = require("../controller/employee.controller");

const employee = (app) => {
  app.get("/api/employee/getlist", CheckToken(), controller.getlist);
  app.get("/api/employee/getone", CheckToken(), controller.getOne);
  app.post("/api/employee/create", CheckToken(), controller.create);
  app.post("/api/employee/setPassword", CheckToken(), controller.setPassword);
  app.post("/api/employee/login", controller.login);
  app.post("/api/employee/refreshtoken", controller.refresh_token);
  app.post("/api/refresh_token", controller.refresh_token);
  app.put("/api/employee/update", CheckToken(), controller.update);
  app.delete("/api/employee/delete", CheckToken(), controller.remove);
};

module.exports = employee;
