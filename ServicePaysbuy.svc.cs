using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfConnectPaysbuy.MBA;
using WcfConnectPaysbuy.MWA;
using WcfConnectPaysbuy.get_ws_dtac_wcf;
using WcfConnectPaysbuy.MBAReference;
using WcfConnectPaysbuy.get_ws_dtacReference;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace WcfConnectPaysbuy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {

        dbAccess dba = new dbAccess();

        getTopping cell01 = new getTopping();
        get_ws_dtac cell02 = new get_ws_dtac();
        
        ResponseData_MBA r = new ResponseData_MBA();
        returnMeaRequest[] datalist = new returnMeaRequest[0];

        ResponseData_MBA_confirm rconfirm = new ResponseData_MBA_confirm();
        returnMeaConfirm[] datalistconfirm = new returnMeaConfirm[0];

        ResponseData_MWA rmwaselect = new ResponseData_MWA();
        returnMwaRequest[] dataMwarequest = new returnMwaRequest[0];

        ResponseData_MWA_confirm rconfirm_mwa = new ResponseData_MWA_confirm();
        returnMwaConfirm[] datalistconfirm_mwa = new returnMwaConfirm[0];

        ResponseData_get_ws_dtac rget_ws_dtac = new ResponseData_get_ws_dtac();
        returnPostpaidRequest[] datalistget_ws_dtac = new returnPostpaidRequest[0];

        ResponseData_get_ws_dtac_confirm rget_ws_dtac_confirm = new ResponseData_get_ws_dtac_confirm();
        returnPostpaidConfirm[] datalistget_ws_data_confirm = new returnPostpaidConfirm[0];


        public ResponseData_MBA mea_request_query(RequestData_MBA obj)// ดูข้อมูลใน MBA
        {
            try
            {
                r.Obj = new DataContract_MBA();
                obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                //obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                obj.contractAC = "013486887";

                datalist = cell01.mea_request_query(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                    obj.tid == null ? "" : obj.tid.Trim(),
                                                    obj.contractAC == null ? "" : obj.contractAC.Trim(),
                                                    obj.email == null ? "" : obj.email.Trim(),
                                                    obj.mobileNo == null ? "" : obj.mobileNo.Trim());
                r.Obj.tid = datalist[0].tid;
                r.Obj.respCode = datalist[0].respCode;
                r.Obj.respMessage = datalist[0].respMessage;
                r.Obj.name = datalist[0].name;
                r.Obj.address = datalist[0].address;
                r.Obj.dueDate = datalist[0].dueDate;
                r.Obj.amount = datalist[0].amount;
                r.Obj.invoiceDate = datalist[0].invoiceDate;
                r.Obj.discountAmount = datalist[0].discountAmount;
                r.Obj.ref1 = datalist[0].ref1;
                r.Obj.refId = datalist[0].refId;
            }
            catch
            {
                r.Obj.tid = obj.tid == null ? "" : obj.tid.Trim();
                r.Obj.respCode = "999";
                r.Obj.respMessage = "Server Error";
                r.Obj.name = "";
                r.Obj.address = "";
                r.Obj.dueDate = "";
                r.Obj.amount = "";
                r.Obj.invoiceDate = "";
                r.Obj.discountAmount = "";
                r.Obj.ref1 = "";
                r.Obj.refId = "";
            }

            return r;
        }

        public ResponseData_MBA_confirm mea_request_confirm(RequestData_MBA_confirm obj) //confirm การชำระเงิน Cellue
        {
            try
            {
                rconfirm.Obj = new DataContract_MBA_confirm();
                //obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                ////obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                datalistconfirm = cell01.mea_request_confirm(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                            obj.tid == null ? "" : obj.tid.Trim(),
                                                            obj.refID == null ? "" : obj.refID.Trim(),
                                                            Convert.ToDouble(obj.amt) == 0 ? 0 : obj.amt,
                                                            Convert.ToDouble(obj.fee) == 0 ? 0 : obj.fee,
                                                            obj.confirmStatus == null ? "" : obj.confirmStatus.Trim(),
                                                            obj.cardsNo == null ? "" : obj.cardsNo.Trim(),
                                                            obj.origBankTrxId == null ? "" : obj.origBankTrxId.Trim());

                

                if (datalistconfirm[0].refId != "" || (obj.origBankTrxId == null ? "" : obj.origBankTrxId.Trim()) != ""
                    || (obj.clientId == null ? "" : obj.clientId.Trim()) != "")
                {
                    using (var conn = dba.dbConnect())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand("updata_psb_mea_api_cf_operation_wcf", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 300; // Increase this to allow the proc longer to run
                            cmd.Parameters.AddWithValue("@refID", obj.refID);
                            cmd.Parameters.AddWithValue("@bankTrxId", obj.origBankTrxId == null ? "" : obj.origBankTrxId.Trim());
                            cmd.Parameters.AddWithValue("@clientId", obj.clientId == null ? "" : obj.clientId.Trim());

                            SqlDataReader dr = cmd.ExecuteReader();
                            dr.Close();
                        }
                        conn.Close();
                    }
                }
                else
                {
                    rconfirm.Obj.refId = "";
                    rconfirm.Obj.respCode = "9902";
                    rconfirm.Obj.respMessage = "Invalid null";
                    rconfirm.Obj.tid = "";
                    return rconfirm;
                }

                rconfirm.Obj.refId = datalistconfirm[0].refId;
                rconfirm.Obj.respCode = datalistconfirm[0].respCode;
                rconfirm.Obj.respMessage = datalistconfirm[0].respMessage;
                rconfirm.Obj.tid = datalistconfirm[0].tid;

            }
            catch
            {
                rconfirm.Obj.refId = "";
                rconfirm.Obj.respCode = "999";
                rconfirm.Obj.respMessage = "Server Error";
                rconfirm.Obj.tid = obj.tid == null ? "" : obj.tid.Trim();
            }
            return rconfirm;
        }

        public ResponseData_MWA mwa_request_query(RequestData_MWA obj) //เก็บข้อมุล MWA
        {
            rmwaselect.Obj = new  DataContract_MWA();
            try
            {
                obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                //obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                dataMwarequest = cell01.mwa_request_query(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                          obj.tid == null ? "" : obj.tid.Trim(),
                                                          obj.taxID == null ? "" : obj.taxID.Trim(),
                                                          obj.suffix == null ? "" : obj.suffix.Trim(),
                                                          obj.ref1 == null ? "" : obj.ref1.Trim(),
                                                          obj.ref2 == null ? "" : obj.ref2.Trim(),
                                                          obj.amount == 0 ? 0 : obj.amount,
                                                          obj.name == null ? "" : obj.name.Trim(),
                                                          obj.address == null ? "" : obj.address,
                                                          obj.email == null ? "" : obj.email.Trim(),
                                                          obj.mobileNo == null ? "" : obj.mobileNo.Trim());
                rmwaselect.Obj.tid = dataMwarequest[0].tid;
                rmwaselect.Obj.refId = dataMwarequest[0].refId;
                rmwaselect.Obj.respCode = dataMwarequest[0].respCode;
                rmwaselect.Obj.respMessage = dataMwarequest[0].respMessage;
            }
            catch
            {
                rmwaselect.Obj.tid = obj.tid == null ? "" : obj.tid.Trim();
                rmwaselect.Obj.refId = "";
                rmwaselect.Obj.respCode = "999";
                rmwaselect.Obj.respMessage = "Server Error";
            }

            return rmwaselect;
        }

        public ResponseData_MWA_confirm mwa_request_confirm(RequestData_MWA_confirm obj)
        {
            try
            {
                obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                //obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                rconfirm_mwa.Obj = new DataContract_MWA_confirm();
                datalistconfirm_mwa = cell01.mwa_request_confirm(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                                 obj.tid == null ? "" : obj.tid.Trim(),
                                                                 obj.refID == null ? "" : obj.refID.Trim(),
                                                                 obj.amount == 0 ? 0 : obj.amount,
                                                                 obj.fee == 0 ? 0 : obj.fee,
                                                                 obj.confirmStatus == null ? "" : obj.confirmStatus,
                                                                 obj.cardsNo == null ? "" : obj.cardsNo,
                                                                 obj.bankTrxid == null ? "" : obj.bankTrxid);

                string clientId = obj.clientId == null ? "" : obj.clientId.Trim();

                if (datalistconfirm[0].refId != "" || (obj.bankTrxid == null ? "" : obj.bankTrxid) != "" ||
                    (obj.clientId == null ? "" : obj.clientId.Trim()) != "")
                {
                    using (var conn = dba.dbConnect())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand("updata_psb_mwa_api_cf_operation_wcf", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 300; // Increase this to allow the proc longer to run
                            cmd.Parameters.AddWithValue("@refID", datalistconfirm_mwa[0].refId);
                            cmd.Parameters.AddWithValue("@bankTrxId", obj.bankTrxid == null ? "" : obj.bankTrxid);
                            cmd.Parameters.AddWithValue("@clientId", obj.clientId == null ? "" : obj.clientId.Trim());

                            SqlDataReader dr = cmd.ExecuteReader();
                            dr.Close();
                        }
                        conn.Close();
                    }
                }
                else
                {
                    rconfirm_mwa.Obj.refId = "";
                    rconfirm_mwa.Obj.respCode = "9902";
                    rconfirm_mwa.Obj.respMessage = "Invalid null";
                    rconfirm_mwa.Obj.tid = "";
                    return rconfirm_mwa;
                }

                rconfirm_mwa.Obj.tid = datalistconfirm_mwa[0].tid;
                rconfirm_mwa.Obj.refId = datalistconfirm_mwa[0].refId;
                rconfirm_mwa.Obj.respCode = datalistconfirm_mwa[0].respCode;
                rconfirm_mwa.Obj.respMessage = datalistconfirm_mwa[0].respMessage;
            }
            catch
            {
                rconfirm_mwa.Obj.tid = obj.tid == null ? "" : obj.tid.Trim();
                rconfirm_mwa.Obj.refId = "";
                rconfirm_mwa.Obj.respCode = "999";
                rconfirm_mwa.Obj.respMessage = "Server Error";
            }
            return rconfirm_mwa;
        } // confirm การชำระเงิน 

        public ResponseData_get_ws_dtac postpaid_request_query(RequestData_get_ws_dtac obj)
        {
            try
            {
                obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                //obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                rget_ws_dtac.Obj = new DataContract_get_ws_dtac();
                datalistget_ws_dtac = cell02.postpaid_request_query(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                                    obj.paymentChannel == null ? "" : obj.paymentChannel,
                                                                    obj.paymentType == null ? "" : obj.paymentType,
                                                                    obj.tid == null ? "" : obj.tid,
                                                                    obj.amt == 0 ? 0 : obj.amt,
                                                                    obj.mobileNo == null ? "" : obj.mobileNo,
                                                                    obj.email == null ? "" : obj.email);



                rget_ws_dtac.Obj.tid = datalistget_ws_dtac[0].tid;
                rget_ws_dtac.Obj.respCode = datalistget_ws_dtac[0].respCode;
                rget_ws_dtac.Obj.respMessage = datalistget_ws_dtac[0].respMessage;
                rget_ws_dtac.Obj.min = datalistget_ws_dtac[0].min;
                rget_ws_dtac.Obj.max = datalistget_ws_dtac[0].max;
                rget_ws_dtac.Obj.total = datalistget_ws_dtac[0].total;
                rget_ws_dtac.Obj.balance = datalistget_ws_dtac[0].balance;
                rget_ws_dtac.Obj.reconnect = datalistget_ws_dtac[0].reconnect;
                rget_ws_dtac.Obj.refId = datalistget_ws_dtac[0].refId;
            }
            catch
            {
                rget_ws_dtac.Obj.tid = obj.tid == null ? "" : obj.tid;
                rget_ws_dtac.Obj.respCode = "999";
                rget_ws_dtac.Obj.respMessage = "Server Error";
                rget_ws_dtac.Obj.min = "0";
                rget_ws_dtac.Obj.max = "0";
                rget_ws_dtac.Obj.total = "0";
                rget_ws_dtac.Obj.balance = "0";
                rget_ws_dtac.Obj.reconnect = "";
                rget_ws_dtac.Obj.refId = "";
            }
            return rget_ws_dtac;
        }

        public ResponseData_get_ws_dtac_confirm postpaid_request_confirm(RequestData_get_ws_dtac_confirm obj)
        {
            try
            {
                obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                //obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                rget_ws_dtac_confirm.Obj = new DataContract_get_ws_dtac_confirm();
                datalistget_ws_data_confirm = cell02.postpaid_request_confirm(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                                              obj.tid == null ? "" : obj.tid,
                                                                              obj.refID == null ? "" : obj.secureCode.Trim(),
                                                                              obj.amt == 0 ? 0 : obj.amt,
                                                                              obj.confirmStatus == null ? "" : obj.confirmStatus,
                                                                              obj.bankTrxid == null ? "" : obj.bankTrxid);
                                                                             // obj.clientId)

                if (datalistconfirm[0].refId != "" || (obj.bankTrxid == null ? "" : obj.bankTrxid.Trim()) != ""
                    || (obj.clientId == null ? "" : obj.clientId.Trim()) != "")
                {
                    using (var conn = dba.dbConnect())
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand("updata_psb_get_ws_dtac_api_cf_operation_wcf", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 300; // Increase this to allow the proc longer to run
                            cmd.Parameters.AddWithValue("@refID", datalistconfirm[0].refId);
                            cmd.Parameters.AddWithValue("@bankTrxId", obj.bankTrxid == null ? "" : obj.bankTrxid.Trim());
                            cmd.Parameters.AddWithValue("@clientId", obj.clientId == null ? "" : obj.clientId.Trim());

                            SqlDataReader dr = cmd.ExecuteReader();
                            dr.Close();
                        }
                        conn.Close();
                    }
                }
                else
                {
                    rget_ws_dtac_confirm.Obj.refId = "";
                    rget_ws_dtac_confirm.Obj.respCode = "9902";
                    rget_ws_dtac_confirm.Obj.respMessage = "Invalid null";
                    rget_ws_dtac_confirm.Obj.tid = "";
                    return rget_ws_dtac_confirm;
                }


                rget_ws_dtac_confirm.Obj.tid = datalistget_ws_data_confirm[0].tid;
                rget_ws_dtac_confirm.Obj.respCode = datalistget_ws_data_confirm[0].respCode;
                rget_ws_dtac_confirm.Obj.respMessage = datalistget_ws_data_confirm[0].respMessage;
                rget_ws_dtac_confirm.Obj.refId = datalistget_ws_data_confirm[0].refId;
    

            }
            catch
            {
                rget_ws_dtac_confirm.Obj.tid = obj.tid == null ? "" : obj.tid;
                rget_ws_dtac_confirm.Obj.respCode = "999";
                rget_ws_dtac_confirm.Obj.respMessage = "Server Error";
                rget_ws_dtac_confirm.Obj.refId = "";
            }
            return rget_ws_dtac_confirm;
        }
       
    }
}
