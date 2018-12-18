<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 25.11.2016
  Time: 15:38
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Products</title>
</head>
<body>
<c:url var="addProductUrl" value="/products/add"/>
<c:if test="${empty products}">
    <div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div mainEmptyAll_div" id="emptyAllProductsView_div"><h1 align="center">Products are empty</h1></div>
    <div align="center"><a class="emptyAllAddEditEmptySalesDiscountStoryStatisticsView_a_input allAddEditEmptySalesDiscountStoryStatisticsView_a_input emptyAllDiscountStoryStatisticsView_a" id="emptyAllProductsView_a" href="${addProductUrl}">Add product</a></div>
</c:if>
<c:if test="${!empty products}">
    <h1 align="center">Products</h1>
    <table class="allAddViewProductSalesSaleProducts_table" width="1224" border="1">
        <thead>
        <tr>
            <th><label>Name</label></th>
            <th><label>Price</label></th>
            <th colspan="3"><label>Actions</label></th>
        </tr>
        </thead>
        <tbody>
        <c:forEach items="${products}" var="product">
            <tr>
                <c:url var="editProductUrl" value="/products/edit?name=${product.name}"/>
                <c:url var="productSalesUrl" value="/products/sales?name=${product.name}&wRP=false"/>
                <c:url var="productSalesWRPUrl" value="/products/sales?name=${product.name}&wRP=true"/>
                <th><div class="allView_c_out"><c:out value="${product.name}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${product.price}"/></div></th>
                <th class="allView_th" width="54"><a href="${editProductUrl}">Edit</a></th>
                <th class="allView_th" width="68"><a href="${productSalesUrl}">Sales</a></th>
                <th class="allView_th" width="124"><a href="${productSalesWRPUrl}">SalesWRP</a></th>
            </tr>
        </c:forEach>
        </tbody>
    </table>
    <div class="allAddView_div" align="center"><a class="allAddView_a" href="${addProductUrl}">Add product</a></div>
    <div class="allAddView_div" align="center"><a class="allAddView_a" href="/">Main page</a></div>
</c:if>
</body>
</html>
