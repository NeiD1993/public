<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 25.11.2016
  Time: 15:44
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../style/style.css"%></style>
    <title>Discount story</title>
</head>
<body>
<c:if test="${empty allDiscountStory}">
    <div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div mainEmptyAll_div" id="emptyAllDiscountStoryView_div"><h1 align="center">Discount story is empty</h1></div>
    <div align="center"><a class="emptyAllAddEditEmptySalesDiscountStoryStatisticsView_a_input allAddEditEmptySalesDiscountStoryStatisticsView_a_input emptyAllDiscountStoryStatisticsView_a emptyAllDiscountStoryStatisticsView_a_size" href="/">Main page</a></div>
</c:if>
<c:if test="${!empty allDiscountStory}">
    <h1 align="center">Discount story</h1>
    <table class="allAddViewProductSalesSaleProducts_table" width="1380" border="1">
        <thead>
        <tr>
            <th width="350"><label>Begin date</label></th>
            <th width="350"><label>End date</label></th>
            <th width="400"><label>Product</label></th>
            <th><label>Discount (%)</label></th>
        </tr>
        </thead>
        <tbody>
        <c:forEach items="${allDiscountStory}" var="discountStory">
            <tr>
                <th><div class="allView_c_out"><c:out value="${discountStory.beginDate}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${discountStory.endDate}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${discountStory.product.name}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${discountStory.discount}"/></div></th>
            </tr>
        </c:forEach>
        </tbody>
    </table>
    <div class="allAddView_div" align="center"><a class="allAddView_a" href="/">Main page</a></div>
</c:if>
</body>
</html>
