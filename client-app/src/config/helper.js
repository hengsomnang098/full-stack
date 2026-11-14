import dayjs from "dayjs";
import axios from "axios";

export const Config = {
  base_url: "http://localhost:8081/api/",
  image_path: "http://localhost:8081/uploads/",
  version: "",
};

export const isEmptyOrNull = (value) => {
  if (
    value === "" ||
    value === null ||
    value === undefined ||
    value === "null" ||
    value === "undefined"
  ) {
    return true;
  }
  return false;
};

export const getUser = () => {
  var user = localStorage.getItem("profile");
  if (user != null && user != "") {
    user = JSON.parse(user);
    return user;
  }
  return null;
};

export const setUser = (user = {}) => {
  localStorage.setItem("profile", JSON.stringify(user));
  localStorage.setItem("isLogin", "1");
};

export const logout = async () => {
  try {
    await axios.post(
      Config.base_url + "employee/logout",
      {},
      { withCredentials: true }
    );
  } catch {
    // ignore — clear local session anyway
  }
  localStorage.setItem("profile", "");
  localStorage.setItem("isLogin", "0");
  localStorage.removeItem("access_token");
  localStorage.removeItem("refresh_token");
  window.location.href = "login";
};

export const isLogin = () => {
  return localStorage.getItem("isLogin") == "1";
};

export const formartDateClient = (date) => {
  if (date !== null && date !== "") {
    return dayjs(date).format("YYYY-MM-DD");
  }
  return null;
};

export const formartDateServer = (date) => {
  if (date !== null && date !== "") {
    return dayjs(date).format("YYYY-MM-DD");
  }
  return null;
};
