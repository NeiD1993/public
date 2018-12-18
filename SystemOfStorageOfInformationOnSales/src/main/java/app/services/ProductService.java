package app.services;

import app.models.classes.productsSales.ProductDomain;
import app.models.classes.productsSales.ProductSaleDomain;
import app.services.base.GetableService;
import org.hibernate.Hibernate;
import org.hibernate.Session;
import org.hibernate.query.Query;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.annotation.Resource;
import java.util.List;

/**
 * Created by NeiD on 24.11.2016.
 */
@Service("productService")
@Transactional
public class ProductService extends GetableService<String, ProductDomain> {

    @Resource(name = "productSaleService")
    private ProductSaleService productSaleService;

    protected String getClassName() {
        return ProductDomain.class.getSimpleName();
    }

    protected ProductDomain getDomain(Session session, String id) {
        return session.get(ProductDomain.class, id);
    }

    public void edit(ProductDomain product) {
        logger.debug("Editing existing " + getClassName());
        Session session = sessionFactory.getCurrentSession();
        ProductDomain existingProduct = getDomain(session, product.getName());
        existingProduct.setPrice(product.getPrice());
        session.update(existingProduct);
    }

    public Float getProductPrice(String name) {
        logger.debug("Retrieving " + getClassName() + " price");
        Query query = sessionFactory.getCurrentSession().createQuery("SELECT price FROM ProductDomain WHERE name = :name");
        query.setParameter("name", name);
        return (Float)query.uniqueResult();
    }

    public List<ProductSaleDomain> getSales(String name) {
        logger.debug("Retrieving sales positions of " + getClassName());
        return productSaleService.getForProduct(name);
    }

    public List<ProductSaleDomain> getSalesWRP(String name) {
        logger.debug("Retrieving repeating sales positions of " + getClassName());
        List<ProductSaleDomain> sales = get(name).getSales();
        Hibernate.initialize(sales);
        return sales;
    }

    public List<String> getAllProductNames() {
        logger.debug("Retrieving all " + getClassName() + "s names");
        Query query = sessionFactory.getCurrentSession().createQuery("SELECT name FROM ProductDomain");
        return query.list();
    }

}
