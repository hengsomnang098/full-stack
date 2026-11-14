import axios from "axios";
import { Config, logout, setUser } from "./helper";
import { message } from "antd";

axios.defaults.withCredentials = true;

export const request = async (url = "", method = "get", data = {}) => {
  var param_get = "";
  if (method === "get" && Object.keys(data).length > 0) {
    Object.keys(data).map((key, i) => {
      param_get += (i == 0 ? "?" : "&") + key + "=" + data[key];
    });
  }

  var headers = { "Content-Type": "application/json" };
  if (data instanceof FormData) {
    headers = { "Content-Type": "multipart/form-data" };
  }

  return axios({
    url: Config.base_url + url + param_get,
    method: method,
    data: data,
    headers,
    withCredentials: true,
  })
    .then((res) => {
      return res.data;
    })
    .catch(async (error) => {
      console.log("Unexpected Error:", error);
      var status = error.response?.status;
      if (status == 404) {
        message.error(error.message + "");
      } else if (status == 500) {
        message.error(error.message + "");
      } else if (status == 401) {
        const refreshed = await refreshToken();
        if (refreshed) {
          return request(url, method, data);
        }
        logout();
        return false;
      }
      return false;
    });
};

export const refreshToken = async () => {
  try {
    const res = await axios({
      url: Config.base_url + "employee/refreshtoken",
      method: "post",
      data: {},
      withCredentials: true,
    });
    if (res?.data?.user) {
      setUser(res.data.user);
    }
    return true;
  } catch (error) {
    message.error("Session expired. Please login again.");
    return false;
  }
};
