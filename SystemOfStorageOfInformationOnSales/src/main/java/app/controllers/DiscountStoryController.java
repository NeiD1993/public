package app.controllers;

import app.controllers.base.BaseDomainController;
import app.models.classes.BeginDateEndDates.BeginDateEndDateModelPrimaryKey;
import app.models.classes.BeginDateEndDates.DiscountStoryDomain;
import app.models.classes.productsSales.ProductDomain;
import app.services.DiscountStoryService;
import app.services.ProductService;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.servlet.ModelAndView;

import javax.annotation.Resource;
import java.util.Date;
import java.util.List;
import java.util.Random;

/**
 * Created by NeiD on 26.11.2016.
 */
@Controller
@RequestMapping("/discountStory")
public class DiscountStoryController extends BaseDomainController<BeginDateEndDateModelPrimaryKey, DiscountStoryDomain> {

    private final static Byte minimalDiscount = 5;

    private final static Byte maximalDiscount = 10;

    public final static Long discountValidPeriod = 20000l;

    @Resource(name = "productService")
    private ProductService productService;

    @Resource(name = "discountStoryService")
    private DiscountStoryService discountStoryService;

    @Scheduled(fixedDelayString = "#{discountStoryController.discountValidPeriod}")
    public void scheduledAddDiscount() {
        logger.debug("Received request to add new incidentally discount");
        Long currentTime = System.currentTimeMillis();
        List<ProductDomain> products = productService.getAll();
        if (!products.isEmpty()) {
            Random indexGenerator = new Random();
            ProductDomain incidentalProduct = products.get(indexGenerator.nextInt(products.size()));
            Byte productDiscount = (byte)(minimalDiscount + Math.random() * ((maximalDiscount - minimalDiscount) + 1));
            postAddDomain("discount", new DiscountStoryDomain(new Date(currentTime),
                    new Date(currentTime + discountValidPeriod), incidentalProduct, productDiscount), discountStoryService);
        }
    }

    @RequestMapping(method = RequestMethod.GET)
    public ModelAndView getAllDiscountStory() {
        return getAllDomains("allDiscountStoryView", "allDiscountStory", discountStoryService);
    }

}
