using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfConnectPaysbuy.MBA;
using WcfConnectPaysbuy.MWA;
using WcfConnectPaysbuy.MBAReference;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using System.IO;

namespace WcfConnectPaysbuy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        
        getTopping cell01 = new getTopping();
        
        ResponseData_MBA r = new ResponseData_MBA();
        returnMeaRequest[] datalist = new returnMeaRequest[0];

        ResponseData_MBA_confirm rconfirm = new ResponseData_MBA_confirm();
        returnMeaConfirm[] datalistconfirm = new returnMeaConfirm[0];

        ResponseData_MWA rmwaselect = new ResponseData_MWA();
        returnMwaRequest[] dataMwarequest = new returnMwaRequest[0];

        ResponseData_MWA_confirm rconfirm_mwa = new ResponseData_MWA_confirm();
        returnMwaConfirm[] datalistconfirm_mwa = new returnMwaConfirm[0];


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
                obj.secureCode = "5D0890DB01B61C57700C64BFA324D072";
                //obj.secureCode = "2BCB0C3A9ACFD36E408905275E6C2860";
                datalistconfirm = cell01.mea_request_confirm(obj.secureCode == null ? "" : obj.secureCode.Trim(),
                                                            obj.tid == null ? "" : obj.tid.Trim(),
                                                            obj.refID == null ? "" : obj.refID.Trim(),
                                                            Convert.ToDouble(obj.amt) == 0 ? 0 : obj.amt,
                                                            Convert.ToDouble(obj.fee) == 0 ? 0 : obj.fee,
                                                            obj.confirmStatus == null ? "" : obj.confirmStatus.Trim(),
                                                            obj.cardsNo == null ? "" : obj.cardsNo.Trim(),
                    // obj.clientId == null ? "" : obj.clientId.Trim(),
                                                            obj.origBankTrxId == null ? "" : obj.origBankTrxId.Trim());

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
                rmwaselect.Obj.respCode = "";
                rmwaselect.Obj.respMessage = "";
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
                //obj.clientId
                //public string origBankTrxId;

                rconfirm_mwa.Obj.tid = datalistconfirm_mwa[0].tid;
                rconfirm_mwa.Obj.refId = datalistconfirm_mwa[0].refId;
                rconfirm_mwa.Obj.respCode = datalistconfirm_mwa[0].respCode;
                rconfirm_mwa.Obj.respMessage = datalistconfirm_mwa[0].respMessage;
            }
            catch
            {
                rconfirm_mwa.Obj.tid = obj.tid == null ? "" : obj.tid.Trim();
                rconfirm_mwa.Obj.refId = "";
                rconfirm_mwa.Obj.respCode = "";
                rconfirm_mwa.Obj.respMessage = "";
            }
            return rconfirm_mwa;
        } // confirm การชำระเงิน 



       
    }
}
