package app.services;

import app.models.classes.productsSales.SaleDomain;
import app.services.base.GetableService;
import org.hibernate.Hibernate;
import org.hibernate.Session;
import org.hibernate.query.Query;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.Date;
import java.util.List;

/**
 * Created by NeiD on 23.11.2016.
 */
@Service("saleService")
@Transactional
public class SaleService extends GetableService<Integer, SaleDomain> {

    protected String getClassName() {
        return SaleDomain.class.getSimpleName();
    }

    protected SaleDomain getDomain(Session session, Integer id) {
        return session.get(SaleDomain.class, id);
    }

    public SaleDomain getWithProducts(Integer id) {
        logger.debug("Retrieving " + getClassName() + " with Products");
        SaleDomain sale = get(id);
        Hibernate.initialize(sale.getProducts());
        return sale;
    }

    public List<SaleDomain> getSalesFromUpdatingPeriod(Date beginUpdatingDate, Date endUpdatingDate) {
        logger.debug("Retrieving " + getClassName() + "s from updating period");
        Query query = sessionFactory.getCurrentSession().createQuery("FROM SaleDomain WHERE date >= :beginUpdatingDate " +
                                                                     "AND date <= :endUpdatingDate");
        query.setParameter("beginUpdatingDate", beginUpdatingDate);
        query.setParameter("endUpdatingDate", endUpdatingDate);
        return query.list();
    }

}
