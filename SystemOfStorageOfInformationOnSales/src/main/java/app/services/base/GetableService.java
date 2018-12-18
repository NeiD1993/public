package app.services.base;

import app.models.IDomain;
import org.hibernate.Session;
import org.springframework.transaction.annotation.Transactional;

/**
 * Created by NeiD on 21.11.2016.
 */
@Transactional
public abstract class GetableService<IdType, DomainType extends IDomain> extends AddedService<IdType, DomainType> {

    protected abstract DomainType getDomain(Session session, IdType id);

    public DomainType get(IdType id) {
        logger.debug("Retrieving " + getClassName());
        DomainType domain = getDomain(sessionFactory.getCurrentSession(), id);
        return domain;
    }

}
