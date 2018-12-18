<%--
  Created by IntelliJ IDEA.
  User: NeiD
  Date: 26.11.2016
  Time: 14:49
  To change this template use File | Settings | File Templates.
--%>
<%@ taglib uri="http://java.sun.com/jsp/jstl/core" prefix="c" %>
<%@ taglib uri="http://www.springframework.org/tags/form" prefix="f" %>
<%@ page contentType="text/html;charset=UTF-8" language="java" %>
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html" charset="UTF-8"/>
    <style><%@include file="../../style/style.css"%></style>
    <title>Add Sale</title>
</head>
<body>
<c:url var="addSaleConfirmUrl" value="/sales/add/confirm"/>
<h1 align="center">Sale</h1>
<f:form modelAttribute="productSalesWrapper" method="post" action="${addSaleConfirmUrl}">
    <table class="allAddViewProductSalesSaleProducts_table" width="1004" border="1">
        <thead>
        <tr>
            <th id="addSaleView_th_position"><label>Position</label></th>
            <th id="addSaleView_th_product"><label>Product</label></th>
            <th><label>Count</label></th>
        </tr>
        </thead>
        <tbody>
        <c:forEach items="${productSalesWrapper.productSales}" varStatus="status">
            <tr>
                <th><div class="allView_c_out"><c:out value="${status.index + 1}"/></div></th>
                <th>
                    <f:select id="addSaleView_select" path="productSales[${status.index}].product.name">
                        <f:option value=""/>
                        <f:options items="${productsNames}"/>
                    </f:select>
                </th>
                <th><input type="number" required min="1" step="1" name="productSales[${status.index}].count"/></th>
            </tr>
        </c:forEach>
        </tbody>
    </table>
    <div id="addSaleView_div" align="center"><input class="addEditView_input addSaleView_input" type="submit" value="Confirm"/></div>
</f:form>
</body>
</html>
