package app.models.classes.factory;

import app.models.classes.productsSales.ProductSaleDomain;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by NeiD on 30.11.2016.
 */
public class ProductSaleDomainFactory {

    public static List<ProductSaleDomain> initializeProductSales(Integer productSalesCount) {
        List<ProductSaleDomain> productSales = new ArrayList<ProductSaleDomain>();
        for (Integer i = 0; i < productSalesCount; i++)
            productSales.add(new ProductSaleDomain());
        return productSales;
    }

}
