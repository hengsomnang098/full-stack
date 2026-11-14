const product_controller = require("../controller/product.controller");
const { upload } = require("../config/helper");
const { CheckToken } = require("../controller/employee.controller");

const product = (app) => {
  app.get("/api/product/getList", CheckToken(), product_controller.getList);
  app.get("/api/product/getById", CheckToken(), product_controller.getById);
  app.post(
    "/api/product/create",
    CheckToken(),
    upload.single("image"),
    product_controller.create
  );
  app.put(
    "/api/product/update",
    CheckToken(),
    upload.single("image"),
    product_controller.update
  );
  app.delete("/api/product/delete", CheckToken(), product_controller.remove);
};

module.exports = product;
