const category_controller = require("../controller/category.controller");
const { CheckToken } = require("../controller/employee.controller");

const category = (app) => {
  app.get("/api/category", CheckToken(), category_controller.getList);
  app.get("/api/category/getById", CheckToken(), category_controller.getById);
  app.post("/api/category", CheckToken(), category_controller.create);
  app.put("/api/category", CheckToken(), category_controller.update);
  app.delete("/api/category", CheckToken(), category_controller.remove);
};

module.exports = category;
