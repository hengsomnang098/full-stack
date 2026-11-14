const db = require("../config/db");
const { logError, validation } = require("../config/helper");
const bcrypt = require("bcryptjs");
const jwt = require("jsonwebtoken");
const {
  ACCESS_TOKEN_KEY,
  REFRESH_TOKEN_KEY,
  ACCESS_TOKEN_EXPIRES_IN,
  REFRESH_TOKEN_EXPIRES_IN,
} = require("../config/token_key");

const EMPLOYEE_SAFE_COLUMNS =
  "e.Id, e.RoleId, e.Firstname, e.Lastname, e.Gender, e.Dob, e.Tel, e.Email, e.Address, e.Status, e.Image, e.Salary, e.CreateAt, e.CreateBy, e.UpdateAt, e.UpdateBy";

const setPassword = async (req, res) => {
  try {
    var { Tel, Password, ConfirmPassword } = req.body;

    if (validation(Tel) || validation(Password) || validation(ConfirmPassword)) {
      res.status(400).json({
        error: true,
        message: "Tel, Password, and ConfirmPassword are required",
      });
      return;
    }

    if (Password !== ConfirmPassword) {
      res.status(400).json({
        error: true,
        message: "Password and confirm password not match!",
      });
      return;
    }

    // Authenticated users may only reset their own password unless RoleId is admin (1)
    const isAdmin = req.user && Number(req.user.RoleId) === 1;
    if (!isAdmin && req.user && req.user.Tel !== Tel) {
      res.status(403).json({
        error: true,
        message: "You can only set your own password",
      });
      return;
    }

    const [user] = await db.query("SELECT Id FROM employee WHERE Tel=:Tel", {
      Tel: Tel,
    });
    if (user.length == 0) {
      res.status(400).json({
        error: true,
        message: "Unable to set password",
      });
      return;
    }

    const passHash = bcrypt.hashSync(Password, 10);
    const [data] = await db.query(
      "UPDATE employee SET Password=:Password WHERE Tel = :Tel",
      { Password: passHash, Tel: Tel }
    );
    res.json({
      message: data.affectedRows
        ? "Password set successfully"
        : "Something wrong!",
    });
  } catch (err) {
    logError("employee.setPassword", err, res);
  }
};

const login = async (req, res) => {
  try {
    var { Username, Password } = req.body;
    const genericError = {
      error: true,
      message: "Invalid username or password",
    };

    if (validation(Username) || validation(Password)) {
      res.status(400).json(genericError);
      return;
    }

    const [user] = await db.query(
      "SELECT Id, RoleId, Firstname, Lastname, Gender, Dob, Tel, Email, Address, Status, Image, Salary, Password FROM employee WHERE Tel=:Tel",
      { Tel: Username }
    );
    if (user.length == 0) {
      res.status(401).json(genericError);
      return;
    }

    var passwordFromDb = user[0].Password;
    if (!passwordFromDb) {
      res.status(401).json(genericError);
      return;
    }

    var isCorrectPassword = bcrypt.compareSync(Password, passwordFromDb);
    if (isCorrectPassword) {
      delete user[0].Password;
      var access_token = jwt.sign({ data: user[0] }, ACCESS_TOKEN_KEY, {
        expiresIn: ACCESS_TOKEN_EXPIRES_IN,
      });
      var refresh_token = jwt.sign({ data: user[0] }, REFRESH_TOKEN_KEY, {
        expiresIn: REFRESH_TOKEN_EXPIRES_IN,
      });
      res.json({
        message: "Login success",
        user: user[0],
        access_token: access_token,
        refresh_token: refresh_token,
      });
    } else {
      res.status(401).json(genericError);
    }
  } catch (err) {
    logError("employee.login", err, res);
  }
};

const CheckToken = () => {
  return (req, res, next) => {
    var authorization = req.headers.authorization;
    var token_from_client = null;
    if (authorization != null && authorization != "") {
      token_from_client = authorization.split(" ");
      token_from_client = token_from_client[1];
    }
    if (token_from_client == null) {
      res.status(401).send({
        message: "Unauthorized",
      });
    } else {
      jwt.verify(token_from_client, ACCESS_TOKEN_KEY, (error, result) => {
        if (error) {
          res.status(401).send({
            message: "Unauthorized",
            error: error,
          });
        } else {
          req.user = result.data;
          req.user_id = result.data.Id;
          next();
        }
      });
    }
  };
};

const refresh_token = async (req, res) => {
  try {
    var refresh_token = req.body.refresh_token;
    if (validation(refresh_token)) {
      res.status(401).send({
        message: "Unauthorized-Refresh-Token",
      });
    } else {
      jwt.verify(refresh_token, REFRESH_TOKEN_KEY, async (error, result) => {
        if (error) {
          res.status(401).send({
            message: "Unauthorized-Refresh-Token",
            error: error,
          });
        } else {
          var access_token = jwt.sign(
            { data: result.data },
            ACCESS_TOKEN_KEY,
            { expiresIn: ACCESS_TOKEN_EXPIRES_IN }
          );
          var refresh_token_new = jwt.sign(
            { data: result.data },
            REFRESH_TOKEN_KEY,
            { expiresIn: REFRESH_TOKEN_EXPIRES_IN }
          );
          res.json({
            access_token: access_token,
            refresh_token: refresh_token_new,
            // keep legacy key for older clients
            refesh_token: refresh_token_new,
          });
        }
      });
    }
  } catch (err) {
    logError("employee.refreshtoken", err, res);
  }
};

const getlist = async (req, res) => {
  try {
    var { txt_search, status, role_id } = req.query;
    var param = {};
    var sql =
      "SELECT " +
      EMPLOYEE_SAFE_COLUMNS +
      ", r.Name as RoleName FROM employee e INNER JOIN role r ON e.RoleId = r.Id WHERE 1=1 ";

    if (!validation(txt_search)) {
      sql +=
        "AND (e.Firstname LIKE :txt_search OR e.Lastname LIKE :txt_search) ";
      param["txt_search"] = "%" + txt_search + "%";
    }
    if (!validation(status)) {
      sql += " AND e.Status =:status";
      param["status"] = status;
    }
    if (!validation(role_id)) {
      if (role_id == "All") {
        param["role_id"] = role_id;
      } else {
        sql += " AND e.RoleId =:role_id";
        param["role_id"] = role_id;
      }
    }
    sql += " ORDER BY Id DESC";
    const [list] = await db.query(sql, param);
    const [role] = await db.query("SELECT * FROM role");
    res.json({
      list: list,
      role: role,
      user_id: req.user,
    });
  } catch (err) {
    logError("employee.getlist", err, res);
  }
};

const getOne = async (req, res) => {
  try {
    var param = {
      Id: req.body.Id || req.query.Id,
    };
    var sql =
      "SELECT Id, RoleId, Firstname, Lastname, Gender, Dob, Tel, Email, Address, Status, Image, Salary, CreateAt, CreateBy, UpdateAt, UpdateBy FROM employee WHERE Id=:Id";
    const [data] = await db.query(sql, param);
    res.json({
      data: data,
    });
  } catch (error) {
    logError("employee.getList", error, res);
  }
};

const create = async (req, res) => {
  try {
    var sql = `
      INSERT INTO employee  (RoleId,Firstname, Lastname, Gender, Dob, Tel, Email, Address, Status,Image,Salary) 
      VALUES (:RoleId,:Firstname, :Lastname, :Gender, :Dob, :Tel, :Email, :Address, :Status,:Image,:Salary)`;
    var param = {
      RoleId: req.body.RoleId,
      Firstname: req.body.Firstname,
      Lastname: req.body.Lastname,
      Gender: req.body.Gender,
      Dob: req.body.Dob,
      Tel: req.body.Tel,
      Email: req.body.Email,
      Address: req.body.Address,
      Status: req.body.Status,
      Image: null,
      Salary: req.body.Salary,
    };
    const [data] = await db.query(sql, param);
    res.json({
      message: "Create success",
      data: data,
    });
  } catch (err) {
    logError("employee.create", err, res);
  }
};

const update = async (req, res) => {
  try {
    var sql =
      "UPDATE employee SET RoleId=:RoleId ,Firstname=:Firstname, Lastname=:Lastname, Gender=:Gender, Dob=:Dob, Tel=:Tel, Email=:Email, Address=:Address, Status=:Status,Salary=:Salary ,CreateBy=:CreateBy ,UpdateBy=:UpdateBy WHERE Id =:Id";
    var param = {
      Id: req.body.Id,
      RoleId: req.body.RoleId,
      Firstname: req.body.Firstname,
      Lastname: req.body.Lastname,
      Gender: req.body.Gender,
      Dob: req.body.Dob,
      Tel: req.body.Tel,
      Email: req.body.Email,
      Address: req.body.Address,
      Status: req.body.Status,
      Salary: req.body.Salary,
      CreateBy: req.body.CreateBy,
      UpdateBy: req.user_id || req.body.UpdateBy,
    };
    const [data] = await db.query(sql, param);
    res.json({
      message: data.affectedRows != 0 ? "Update success" : "Not found",
      data: data,
    });
  } catch (err) {
    logError("employee.update", err, res);
  }
};

const remove = async (req, res) => {
  try {
    var sql = "DELETE FROM employee WHERE Id = :Id";
    var param = {
      Id: req.body.Id,
    };
    const [data] = await db.query(sql, param);
    res.json({
      message: data.affectedRows != 0 ? "Remove success" : "Not found",
      data: data,
    });
  } catch (err) {
    logError("employee.remove", err, res);
  }
};

module.exports = {
  getlist,
  create,
  update,
  remove,
  getOne,
  setPassword,
  login,
  CheckToken,
  refresh_token,
};
