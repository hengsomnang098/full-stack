import { useEffect, useState, useRef } from "react";
import { request } from "../config/request";
import {
  Table,
  Button,
  Modal,
  Input,
  Form,
  Select,
  message,
  Image,
  Tag,
  InputNumber,
} from "antd";
import { Config, isEmptyOrNull } from "../config/helper";
import MainPage from "../components/page/MainPage";
import { DeleteOutlined } from "@ant-design/icons";
const POSPage = () => {
  const [list, setList] = useState([]);
  // const [category, setCategory] = useState([]);
  const [customer, setCustomer] = useState([]);
  const [paymentMethod, setPaymentMethod] = useState([]);

  const [loading, setLoading] = useState(false);
  // const [open, setOpen] = useState(false);
  const [formCat] = Form.useForm();
  // const [fileSelected, setFileSelected] = useState(null); // past to api
  const [subTotal, setSubTotal] = useState(0);
  const [totalDiscount, setTotalDiscount] = useState(0);
  const [totalToPay, setTtotalToPay] = useState(0);

  useEffect(() => {
    formCat.setFieldsValue({
      Status: "1",
    });
    initInfo();
  }, [formCat]);

  const filterRef = useRef({
    txt_search: "",
  });

  // const fileRef = useRef(null);

  const initInfo = async () => {
    setLoading(true);
    const res = await request("pos/initInfo", "get");
    setLoading(false);
    if (res) {
      setCustomer(res.customer);
      setPaymentMethod(res.order_payment_method);
    }
  };

  const getList = async () => {
    if (isEmptyOrNull(filterRef.current.txt_search)) {
      return;
    }
    setLoading(true);
    var param = {
      txt_search: filterRef.current.txt_search,
    };
    const res = await request("pos/searchProduct", "get", param);
    setLoading(false);

    if (res) {
      if (res.list.length == 0) {
        message.error("Product not Found");
      } else {
        var listTmp = res.list;
        listTmp[0]["QtyOrder"] = 1; //create new key qty
        var indexFind = list.findIndex((item) => item.Id == listTmp[0].Id);
        if (indexFind >= 0) {
          // update qty order with current item
          listTmp = list;
          listTmp[indexFind].QtyOrder = listTmp[indexFind].QtyOrder + 1;
        } else {
          listTmp = [...list, ...listTmp]; //concat arry
        }

        //find total to pay
        //find total discount
        var findSubTotal = 0,
          findTotalDiscountPrice = 0,
          findTotalToPay = 0;

        listTmp.map((item) => {
          findSubTotal += Number(item.QtyOrder * item.Price);
          var Dis = item.Discount == null ? 0 : Number(item.Discount);

          //calculate discount price
          findTotalDiscountPrice +=
            (Number(item.QtyOrder) * item.Price * Dis) / 100;

          //total to pay
          findTotalToPay +=
            Number(item.QtyOrder) * Number(item.Price) - findTotalDiscountPrice;
        });
        setList(listTmp);
        setSubTotal(findSubTotal);
        setTotalDiscount(findTotalDiscountPrice);
        setTtotalToPay(findTotalToPay);
      }
    }
  };
  const onClickBtnDelete = async (item) => {
    Modal.confirm({
      title: "Delete",
      content: "Are you sure you want to delete ?",
      okText: "Yes",
      cancelText: "No",
      okType: "danger",
      centered: true,
      onOk: async () => {
        var data = {
          Id: item.Id,
        };
        const res = await request("pos/delete", "delete", data);
        if (res) {
          message.success(res.message);
          getList();
        }
      },
    });
  };

  const onFinish = async (item) => {
    var param = {
      CustomerId: item.CustomerId,
      PaymentMethodId: item.PaymentMethodId,
      TotalPaid: item.PaidAmount,
      Product: list,
    };

    const res = await request("pos/checkout", "post", param);
    if (res) {
      if (res.message) {
        // var mgs = "";
        // Object.keys(res.message).map((key) => {
        //   mgs += `${key} : ${res.message[key]}`;
        // });
        message.success("Order Sucessfully Added");
        setList([]);
        setTtotalToPay(0);
        setTotalDiscount(0);
        setSubTotal(0);
        formCat.resetFields();
      } else {
        message.error("Something went wrong");
      }

      // getList();
      // onCloseModal();
    }
  };

  const onTextSearch = (e) => {
    filterRef.current.txt_search = e;
    getList();
  };

  return (
    <MainPage loading={loading}>
      <div className="page-toolbar" style={{ marginBottom: 16 }}>
        <div className="txt_title">POS cart</div>
        <Input.Search
          allowClear
          onSearch={onTextSearch}
          placeholder="Scan or search product"
          size="large"
        />
        <Button
          type="default"
          size="large"
          onClick={() => {
            setList([]);
            setSubTotal(0);
            setTotalDiscount(0);
            setTtotalToPay(0);
          }}
        >
          Clear
        </Button>
      </div>

      <div className="pos-layout">
        <div className="table-scroll">
          <Table
            rowKey={"Id"}
            dataSource={list}
            scroll={{ x: 720 }}
            pagination={{
              pageSize: 5,
            }}
            columns={[
              {
                key: "Name",
                title: "Name",
                dataIndex: "Name",
                render: (value, item, index) => {
                  return (
                    <>
                      <div>
                        {index + 1}. {item.Name}
                      </div>
                      <div>
                        <Tag
                          color={item.Qty > 5 ? "green" : "red"}
                          className="w-16 text-center font-bold"
                        >
                          {item.Qty}
                        </Tag>
                      </div>
                      <div>{item.Description}</div>
                    </>
                  );
                },
              },
              {
                key: "QtyOrder",
                title: "Qty",
                dataIndex: "QtyOrder",
                width: 70,
              },
              {
                key: "Price",
                title: "Price",
                dataIndex: "Price",
                width: 90,
                responsive: ["sm"],
              },
              {
                key: "Dis",
                title: "Dis",
                dataIndex: "Discount",
                width: 70,
                responsive: ["md"],
              },
              {
                key: "Total",
                title: "Total",
                width: 90,
                render: (value, items) => {
                  let QtyOrder = items.QtyOrder;
                  let Price = items.Price;
                  let Dis = items.Discount == null ? 0 : items.Discount;
                  let DiscountPrice = (QtyOrder * Price * Dis) / 100;
                  var Total = QtyOrder * Price - DiscountPrice;
                  return Total;
                },
              },
              {
                key: "Image",
                title: "Image",
                dataIndex: "Image",
                responsive: ["lg"],
                render: (value) => {
                  if (value != null && value != "") {
                    return (
                      <Image
                        src={Config.image_path + value}
                        width={64}
                        height={64}
                        style={{ objectFit: "cover", borderRadius: 8 }}
                      />
                    );
                  }
                  return (
                    <div
                      style={{
                        height: 64,
                        width: 64,
                        backgroundColor: "#CBD5E1",
                        borderRadius: 8,
                      }}
                    />
                  );
                },
              },
              {
                key: "Action",
                title: "",
                dataIndex: "Status",
                width: 64,
                render: (value, item) => (
                  <Button
                    onClick={() => onClickBtnDelete(item)}
                    type="primary"
                    danger
                    size="large"
                    aria-label="Remove item"
                  >
                    <DeleteOutlined />
                  </Button>
                ),
              },
            ]}
          />
        </div>

        <div className="pos-checkout">
          <Form form={formCat} layout="vertical" onFinish={onFinish}>
            <Form.Item
              label="Customer"
              name={"CustomerId"}
              rules={[
                {
                  required: true,
                  message: "Please select a customer",
                },
              ]}
            >
              <Select
                placeholder="Select Customer"
                showSearch
                optionFilterProp="label"
                size="large"
                className="w-full"
              >
                {customer.map((item, index) => (
                  <Select.Option label={item.Tel} key={index} value={item.Id}>
                    {item.Firstname} {item.Lastname} {item.Tel}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>

            <Form.Item
              label="Payment Method"
              name={"PaymentMethodId"}
              rules={[
                {
                  required: true,
                  message: "Please select a payment method",
                },
              ]}
            >
              <Select
                placeholder="Select Payment Method"
                showSearch
                optionFilterProp="label"
                size="large"
                className="w-full"
              >
                {paymentMethod.map((item, index) => (
                  <Select.Option label={item.Name} key={index} value={item.Id}>
                    {item.Name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>

            <div className="pos-summary-row">
              <div>Sub total</div>
              <div>{subTotal}$</div>
            </div>
            <div className="pos-summary-row">
              <div>Discount</div>
              <div>{totalDiscount}$</div>
            </div>
            <div className="pos-summary-row pos-summary-row--total">
              <div>Total</div>
              <div>{totalToPay}$</div>
            </div>

            <Form.Item name="PaidAmount" style={{ marginTop: 12 }}>
              <InputNumber
                placeholder="Amount to pay"
                style={{ width: "100%" }}
                size="large"
              />
            </Form.Item>
            <Form.Item style={{ marginBottom: 0 }}>
              <Button type="primary" htmlType="submit" size="large" block>
                Check out
              </Button>
            </Form.Item>
          </Form>
        </div>
      </div>
    </MainPage>
  );
};

export default POSPage;
