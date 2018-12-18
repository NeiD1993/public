<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 25.11.2016
  Time: 15:46
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../style/style.css"%></style>
    <title>Statistics</title>
</head>
<body>
<c:if test="${empty allStatistics}">
    <div class="mainEmptyAllAddEditEmptySalesDiscountStoryStatisticsView_div mainEmptyAll_div" id="emptyAllStatisticsView_div"><h1 align="center">Statistics is empty</h1></div>
    <div align="center"><a class="emptyAllAddEditEmptySalesDiscountStoryStatisticsView_a_input allAddEditEmptySalesDiscountStoryStatisticsView_a_input emptyAllDiscountStoryStatisticsView_a emptyAllDiscountStoryStatisticsView_a_size" href="/">Main page</a></div>
</c:if>
<c:if test="${!empty allStatistics}">
    <h1 align="center">Statistics</h1>
    <table class="allAddViewProductSalesSaleProducts_table" width="1720" border="1">
        <thead>
        <tr>
            <th width="120"><label>Begin date</label></th>
            <th width="100"><label>End date</label></th>
            <th width="150"><label>Cheque count</label></th>
            <th width="130"><label>Cheque cost</label></th>
            <th width="210"><label>Average cheque cost</label></th>
            <th width="140"><label>Discount sum</label></th>
            <th width="220"><label>Discount cheque cost</label></th>
            <th><label>Average discount cheque cost</label></th>
        </tr>
        </thead>
        <tbody>
        <c:forEach items="${allStatistics}" var="statistics">
            <tr>
                <th><div class="allView_c_out"><c:out value="${statistics.beginDate}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.endDate}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.chequeCount}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.chequeCost}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.averageChequeCost}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.discountSum}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.discountChequeCost}"/></div></th>
                <th><div class="allView_c_out"><c:out value="${statistics.averageDiscountChequeCost}"/></div></th>
            </tr>
        </c:forEach>
        </tbody>
    </table>
    <div class="allAddView_div" align="center"><a class="allAddView_a" href="/">Main page</a></div>
</c:if>
</body>
</html>
