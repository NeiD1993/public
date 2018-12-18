package app.controllers.base;

import app.models.IDomain;
import org.springframework.web.servlet.ModelAndView;

/**
 * Created by NeiD on 27.11.2016.
 */
public abstract class BaseDomainGetAddedWithoutAttributesController<IdType, DomainType extends IDomain> extends BaseDomainController<IdType, DomainType> {

    protected ModelAndView getAddDomain(String viewName, String domainName, DomainType domain) {
        logger.debug("Received request to show add " + domainName + " page");
        return new ModelAndView(viewName, domainName, domain);
    }

}
