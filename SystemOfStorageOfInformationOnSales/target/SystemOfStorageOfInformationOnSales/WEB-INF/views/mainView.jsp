<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 25.11.2016
  Time: 11:14
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../style/style.css"%></style>
    <title>Main</title>
</head>
<body>
<c:url var="productsUrl" value="/products"/>
<c:url var="salesUrl" value="/sales"/>
<c:url var="discountStoryUrl" value="/discountStory"/>
<c:url var="statisticsUrl" value="/statistics"/>
<div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div mainEmptyAll_div" id="mainView_div"><h1 align="center">System of storage of information on sales</h1></div>
<table class="mainAddProductView_table" id="mainView_table" width="500" border="1">
    <tr>
        <th><a href="${productsUrl}">Products</a></th>
        <th><a href="${salesUrl}">Sales</a></th>
        <th><a href="${discountStoryUrl}">Discount story</a></th>
        <th><a href="${statisticsUrl}">Statistics</a></th>
    </tr>
</table>
</body>
</html>
