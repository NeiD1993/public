package app.services.base;

import app.models.IDomain;

/**
 * Created by NeiD on 29.11.2016.
 */
public abstract class AddedService<IdType, DomainType extends IDomain> extends BaseService<IdType, DomainType> {

    public void add(DomainType domain) {
        logger.debug("Adding new " + getClassName());
        sessionFactory.getCurrentSession().save(domain);
    }

}
