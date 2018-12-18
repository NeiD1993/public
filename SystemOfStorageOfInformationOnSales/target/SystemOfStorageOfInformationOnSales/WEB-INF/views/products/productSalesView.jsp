<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 27.11.2016
  Time: 23:17
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Product Sales</title>
</head>
<body>
<c:url var="productsUrl" value="/products"/>
<c:if test="${empty sales}">
    <div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div mainEmptyAll_div" id="productEmptySalesView_div"><h1 align="center">Sales for this product are empty</h1></div>
    <div align="center"><a class="emptyAllAddEditEmptySalesDiscountStoryStatisticsView_a_input allAddEditEmptySalesDiscountStoryStatisticsView_a_input" id="productEmptySalesView_a" href="${productsUrl}">Products page</a></div>
</c:if>
<c:if test="${!empty sales}">
    <h1 align="center">Sales for product</h1>
    <table class="allAddViewProductSalesSaleProducts_table" width="900" border="1">
        <thead>
        <tr>
            <th width="180"><label>Id</label></th>
            <th><label>Date</label></th>
            <th><label>Count</label></th>
        </tr>
        </thead>
        <tbody>
        <c:if test="${!wRP}">
            <c:forEach items="${sales}" var="sale">
                <tr>
                    <th><div class="allView_c_out"><c:out value="${sale[0]}"/></div></th>
                    <th><div class="allView_c_out"><c:out value="${sale[1]}"/></div></th>
                    <th><div class="allView_c_out"><c:out value="${sale[2]}"/></div></th>
                </tr>
            </c:forEach>
        </c:if>
        <c:if test="${wRP}">
            <c:forEach items="${sales}" var="productSale">
                <tr>
                    <th><div class="allView_c_out"><c:out value="${productSale.sale.id}"/></div></th>
                    <th><div class="allView_c_out"><c:out value="${productSale.sale.date}"/></div></th>
                    <th><div class="allView_c_out"><c:out value="${productSale.count}"/></div></th>
                </tr>
            </c:forEach>
        </c:if>
        </tbody>
    </table>
    <div class="productSalesSaleProductsView_div_page" align="center"><a class="productSalesSaleProductView_a" href="${productsUrl}">Products page</a></div>
</c:if>
</body>
</html>
