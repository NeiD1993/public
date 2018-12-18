package app.services.base;

import app.models.IDomain;
import org.apache.log4j.Logger;
import org.hibernate.SessionFactory;
import org.hibernate.query.Query;
import org.springframework.transaction.annotation.Transactional;

import javax.annotation.Resource;
import java.util.List;

/**
 * Created by NeiD on 23.11.2016.
 */
@Transactional
public abstract class BaseService<IdType, DomainType extends IDomain> {

    protected static Logger logger = Logger.getLogger("app/services");

    @Resource(name = "sessionFactory")
    protected SessionFactory sessionFactory;

    protected abstract String getClassName();

    public List<DomainType> getAll() {
        String className = getClassName();
        logger.debug("Retrieving all " + className + "s");
        Query query = sessionFactory.getCurrentSession().createQuery("FROM " + className);
        return query.list();
    }

}
