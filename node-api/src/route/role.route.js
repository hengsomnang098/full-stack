const user_controller = require("../controller/role.controller");
const { CheckToken } = require("../controller/employee.controller");

const user = (app) => {
  app.get("/api/role/getList", CheckToken(), user_controller.getList);
  app.post("/api/role/create", CheckToken(), user_controller.create);
  app.put("/api/role/update", CheckToken(), user_controller.update);
  app.delete("/api/role/delete", CheckToken(), user_controller.remove);
};

module.exports = user;
