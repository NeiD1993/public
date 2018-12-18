<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 30.11.2016
  Time: 10:37
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ taglib uri="http://www.springframework.org/tags/form" prefix="f" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Add Sale Confirm</title>
</head>
<body>
<c:url var="addSaleUrl" value="/sales/add"/>
<h1 align="center">Confirm sale</h1>
<f:form modelAttribute="sale" method="post" action="${addSaleUrl}">
    <table class="allAddViewProductSalesSaleProducts_table" width="1600" border="1px">
        <thead>
        <tr>
            <th><label>Position</label></th>
            <th><label>Product</label></th>
            <th><label>Price</label></th>
            <th><label>Count</label></th>
            <th><label>Cost</label></th>
            <th><label>Discount</label></th>
        </tr>
        </thead>
        <tbody>
        <c:forEach items="${sale.products}" varStatus="status">
            <tr>
                <th><div class="allView_c_out"><c:out value="${status.index + 1}"/></div></th>
                <th><f:input id="addSaleViewConfirm_input_product_name" path="products[${status.index}].product.name" readonly="true"/></th>
                <th><f:input path="products[${status.index}].product.price" readonly="true"/></th>
                <th><f:input path="products[${status.index}].count" readonly="true"/></th>
                <th><f:input path="products[${status.index}].cost" readonly="true"/></th>
                <th><f:input path="products[${status.index}].discount" readonly="true"/></th>
            </tr>
        </c:forEach>
        </tbody>
        <tfoot>
        <tr>
            <th><label>Cost: </label></th>
            <th colspan="2"><div class="allView_c_out"><f:input id="addSaleViewConfirm_input_cost" path="cost" readonly="true"/></div></th>
            <th><label>Discount: </label></th>
            <th colspan="2"><div class="allView_c_out"><f:input id="addSaleViewConfirm_input_discount" path="discount" readonly="true"/></div></th>
        </tr>
        </tfoot>
    </table>
    <div id="addSaleView_div" align="center"><input class="addEditView_input addSaleView_input" type="submit" value="Add"/></div>
</f:form>
</body>
</html>
