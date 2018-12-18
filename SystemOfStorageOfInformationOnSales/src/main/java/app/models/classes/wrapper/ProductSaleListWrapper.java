package app.models.classes.wrapper;

import app.models.classes.productsSales.ProductSaleDomain;

import java.util.List;

/**
 * Created by NeiD on 30.11.2016.
 */
public class ProductSaleListWrapper {

    private List<ProductSaleDomain> productSales;

    public ProductSaleListWrapper() {}

    public ProductSaleListWrapper(List<ProductSaleDomain> productSales) {
        setProductSales(productSales);
    }

    public List<ProductSaleDomain> getProductSales() {
        return productSales;
    }

    public void setProductSales(List<ProductSaleDomain> productSales) {
        this.productSales = productSales;
    }

}
