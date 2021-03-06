﻿export default {
    GetRequestListingURL : "/API/RequestListing/FetchModel?page={page}&by={by}&asc={asc}",
    CheckContractURL : "/API/RequestListing/CheckContract",
    CurrentHost : window.location.protocol + '//' + window.location.host,
    GetDocumentURL: "/Document/GetDocument?id={id}",
    SendRequestURL: "/API/RequestListing/CreateRequest",
    ExportRequestURL: "/API/Report/ExportRequests"
}


