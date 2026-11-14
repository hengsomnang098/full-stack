const fs = require("fs").promises;
const moment = require("moment");
const multer = require("multer");
const path = require("path");

const uploadDir = process.env.UPLOAD_DIR || "C:/xampp/htdocs/fullstack/image_g7/";
const uploadMaxBytes = Number(process.env.UPLOAD_MAX_BYTES || 3 * 1024 * 1024);

const logError = async (
  controller = "user.list",
  message = "error message",
  res
) => {
  try {
    const timestamp = moment().format("YYYY-MM-DD HH:mm:ss");
    const pathFile = `./logs/${controller}.txt`;
    const logMessage = `[${timestamp}]  ${message}\n\n`;
    await fs.appendFile(pathFile, logMessage);
  } catch (error) {
    console.error("Error writing to log file:", error);
  }
  res.status(500).send("Internal Server Error");
};

const validation = (value) => {
  if (value == "" || value == null || value == undefined) {
    return true;
  }
  return false;
};

const upload = multer({
  storage: multer.diskStorage({
    destination: function (req, file, callback) {
      callback(null, uploadDir);
    },
    filename: function (req, file, callback) {
      const uniqueSuffix = Date.now() + "-" + Math.round(Math.random() * 1e9);
      const ext = path.extname(file.originalname).toLowerCase();
      const safeExt = [".png", ".jpg", ".jpeg"].includes(ext) ? ext : "";
      callback(null, file.fieldname + "-" + uniqueSuffix + safeExt);
    },
  }),
  limits: {
    fileSize: uploadMaxBytes,
  },
  fileFilter: function (req, file, callback) {
    if (
      file.mimetype != "image/png" &&
      file.mimetype !== "image/jpg" &&
      file.mimetype !== "image/jpeg"
    ) {
      callback(null, false);
    } else {
      callback(null, true);
    }
  },
});

const removeFile = async (fileName) => {
  try {
    await fs.unlink(path.join(uploadDir, fileName));
    return "File delete successfully";
  } catch (err) {
    console.error("Error deleting file:", err);
    throw err;
  }
};

module.exports = { logError, validation, upload, removeFile };
