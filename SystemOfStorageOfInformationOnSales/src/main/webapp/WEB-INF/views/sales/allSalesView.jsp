<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 25.11.2016
  Time: 15:41
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ taglib uri="http://www.springframework.org/tags/form" prefix="f" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Sales</title>
</head>
<body>
<c:url var="addSaleUrl" value="/sales/add"/>
<c:if test="${empty sales}">
    <form action="${addSaleUrl}">
        <div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div mainEmptyAll_div" id="emptyAllSalesView_div"><h1 align="center">Sales are empty</h1></div>
        <div align="center" id="emptyAllSalesView_div_add">
            <input class="addSaleView_input addEditView_input" type="submit" value="Add sale"/>
            <label>with</label>
            <input id="emptyAllSalesView_input_positions" type="number" required min="1" step="1" name="positionCount"/>
            <label for="emptyAllSalesView_input_positions">positions</label>
        </div>
    </form>
</c:if>
<c:if test="${!empty sales}">
    <h1 align="center">Sales</h1>
    <table class="allAddViewProductSalesSaleProducts_table" width="1300" border="1">
        <thead>
        <tr>
            <th><label>Id</label></th>
            <th width="370"><label>Date</label></th>
            <th width="320"><label>Cost</label></th>
            <th width="300"><label>Discount</label></th>
            <th width="120"><label>Action</label></th>
        </tr>
        </thead>
        <tbody>
        <c:forEach items="${sales}" var="sale">
            <tr>
                <c:url var="saleProductsUrl" value="/sales/products?id=${sale.id}"/>
                <th><div class="allView_c_out"><c:out value="${sale.id}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${sale.date}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${sale.cost}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${sale.discount}"/></div></th>
                <th class="allView_th" width="90"><a href="${saleProductsUrl}">Products</a></th>
            </tr>
        </c:forEach>
        </tbody>
    </table>
    <form action="${addSaleUrl}">
        <div align="center" id="allSalesView_div_add">
            <input class="addSaleView_input addEditView_input" type="submit" value="Add sale"/>
            <label>with</label>
            <input id="allSalesView_input_positions" type="number" required min="1" step="1" name="positionCount"/>
            <label for="allSalesView_input_positions">positions</label>
        </div>
    </form>
    <div class="allAddView_div" align="center"><a class="allAddView_a" href="/">Main page</a></div>
</c:if>
</body>
</html>
