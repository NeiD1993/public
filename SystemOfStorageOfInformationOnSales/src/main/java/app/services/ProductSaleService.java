package app.services;

import app.models.classes.productsSales.ProductSaleDomain;
import app.services.base.BaseService;
import org.hibernate.query.Query;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

/**
 * Created by NeiD on 01.12.2016.
 */
@Service("productSaleService")
@Transactional
public class ProductSaleService extends BaseService<Integer, ProductSaleDomain> {
    protected String getClassName() {
        return null;
    }

    public List<ProductSaleDomain> getForProduct(String name) {
        logger.debug("Retrieving " + getClassName() + "s for product");
        Query query = sessionFactory.getCurrentSession().createQuery("SELECT sale.id AS id, sale.date AS date, SUM(productSale.count) AS count " +
                                                                     "FROM ProductSaleDomain productSale WHERE product.name = :name " +
                                                                     "GROUP BY sale.id, sale.date");
        query.setParameter("name", name);
        return query.list();
    }

}
