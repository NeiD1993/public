<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 01.12.2016
  Time: 10:02
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Sale Products</title>
</head>
<body>
<c:url var="salesUrl" value="/sales"/>
<h1 align="center">Products in sale</h1>
<table class="allAddViewProductSalesSaleProducts_table" width="1150" border="1">
    <thead>
    <tr>
        <th><label>Product</label></th>
        <th width="250"><label>Count</label></th>
        <th><label>Cost</label></th>
        <th width="270"><label>Discount</label></th>
    </tr>
    </thead>
    <tbody>
    <c:forEach items="${sale.products}" var="productSale">
        <tr>
            <th><div class="allView_c_out"><c:out value="${productSale.product.name}"/></div></th>
            <th><div class="allView_c_out"><c:out value="${productSale.count}"/></div></th>
            <th><div class="allView_c_out"><c:out value="${productSale.cost}"/></div></th>
            <th><div class="allView_c_out"><c:out value="${productSale.discount}"/></div></th>
        </tr>
    </c:forEach>
    </tbody>
</table>
<div class="productSalesSaleProductsView_div_page" align="center"><a class="productSalesSaleProductView_a" href="${salesUrl}">Sales page</a></div>
</body>
</html>
