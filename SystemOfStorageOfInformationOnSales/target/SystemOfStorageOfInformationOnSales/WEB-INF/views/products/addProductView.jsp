<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 26.11.2016
  Time: 13:50
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ taglib uri="http://www.springframework.org/tags/form" prefix="f" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Add Product</title>
</head>
<body>
<c:url var="addProductUrl" value="/products/add"/>
<div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div" id="addEditProductView_div"><h1 align="center">Product</h1></div>
<f:form modelAttribute="product" method="post" action="${addProductUrl}">
    <table class="mainAddProductView_table" id="addEditProductView_table" width="360" border="1">
        <tr>
            <th><f:label path="name">Name</f:label></th>
            <th><f:input path="name"/></th>
        </tr>
        <tr>
            <th><label for="addProductView_input_price">Price</label></th>
            <th><input id="addProductView_input_price" type="number" required step="any" name="price"/></th>
        </tr>
    </table>
    <input class="emptyAllAddEditEmptySalesDiscountStoryStatisticsView_a_input allAddEditEmptySalesDiscountStoryStatisticsView_a_input addEditView_input" id="addEditProductView_input" type="submit" value="Add"/>
</f:form>
</body>
</html>
