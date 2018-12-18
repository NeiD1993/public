package app.controllers.base;

import app.models.IDomain;
import app.services.base.AddedService;
import app.services.base.BaseService;
import org.springframework.web.servlet.ModelAndView;

/**
 * Created by NeiD on 26.11.2016.
 */
public abstract class BaseDomainController<IdType, DomainType extends IDomain> extends BaseController {

    protected ModelAndView getAllDomains(String viewName, String domainName, BaseService<IdType, DomainType> service) {
        logger.debug("Received request to show all " + domainName);
        return new ModelAndView(viewName, domainName, service.getAll());
    }

    protected void postAddDomain(String domainName, DomainType domain, AddedService<IdType, DomainType> service) {
        logger.debug("Received request to add new " + domainName);
        service.add(domain);
    }

}
