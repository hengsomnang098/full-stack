const ctr_invoice = require("../controller/invoice.controller");
const { CheckToken } = require("../controller/employee.controller");

const invoice = (app) => {
  app.get("/api/invoice", CheckToken(), ctr_invoice.getList);
  app.get("/api/invoice_details/:Id", CheckToken(), ctr_invoice.invoice_details);
};

module.exports = invoice;
