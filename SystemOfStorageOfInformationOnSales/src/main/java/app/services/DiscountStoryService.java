package app.services;

import app.models.classes.BeginDateEndDates.BeginDateEndDateModelPrimaryKey;
import app.models.classes.BeginDateEndDates.DiscountStoryDomain;
import app.services.base.AddedService;
import org.hibernate.query.Query;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

/**
 * Created by NeiD on 25.11.2016.
 */
@Service("discountStoryService")
@Transactional
public class DiscountStoryService extends AddedService<BeginDateEndDateModelPrimaryKey, DiscountStoryDomain> {

    protected String getClassName() {
        return DiscountStoryDomain.class.getSimpleName();
    }

    public DiscountStoryDomain getCurrentDiscount(){
        logger.debug("Retrieving current " + getClassName());
        Query query = sessionFactory.getCurrentSession().createQuery("FROM DiscountStoryDomain WHERE current_timestamp >= beginDate " +
                                                                     "AND current_timestamp < endDate");
        return (DiscountStoryDomain)query.uniqueResult();
    }

}
