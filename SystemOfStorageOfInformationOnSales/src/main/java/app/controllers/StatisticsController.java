package app.controllers;

import app.controllers.base.BaseDomainController;
import app.models.classes.BeginDateEndDates.BeginDateEndDateModelPrimaryKey;
import app.models.classes.BeginDateEndDates.StatisticsDomain;
import app.models.classes.productsSales.SaleDomain;
import app.services.SaleService;
import app.services.StatisticsService;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.servlet.ModelAndView;

import javax.annotation.Resource;
import java.util.Date;
import java.util.List;

/**
 * Created by NeiD on 26.11.2016.
 */
@Controller
@RequestMapping("/statistics")
public class StatisticsController extends BaseDomainController<BeginDateEndDateModelPrimaryKey, StatisticsDomain> {

    private final static Long statisticsControllerCreateTime = System.currentTimeMillis();

    public final static Long statisticsUpdatingPeriod = 20000l;

    @Resource(name = "saleService")
    private SaleService saleService;

    @Resource(name = "statisticsService")
    private StatisticsService statisticsService;

    @Scheduled(fixedDelayString = "#{statisticsController.statisticsUpdatingPeriod}")
    public void scheduledAddStatistics() {
        logger.debug("Received request to add new updating period statistics");
        Long currentTime = System.currentTimeMillis();
        if (currentTime - statisticsUpdatingPeriod >= statisticsControllerCreateTime) {
            Date beginUpdatingDate = new Date(currentTime - statisticsUpdatingPeriod);
            Date endUpdatingDate = new Date(currentTime);
            StatisticsDomain statistics = new StatisticsDomain(beginUpdatingDate, endUpdatingDate);
            statistics.setBeginDate(beginUpdatingDate);
            statistics.setEndDate(endUpdatingDate);
            List<SaleDomain> sales = saleService.getSalesFromUpdatingPeriod(beginUpdatingDate, endUpdatingDate);
            if (!sales.isEmpty()) {
                Float chequeCost = 0f, discountSum = 0f;
                for (SaleDomain sale : sales) {
                    chequeCost += sale.getCost();
                    discountSum += sale.getDiscount();
                }
                Integer saleSize = sales.size();
                Float discountChequeCost = chequeCost - discountSum;
                statistics.setChequeCount(saleSize);
                statistics.setChequeCost(chequeCost);
                statistics.setAverageChequeCost(chequeCost / saleSize);
                statistics.setDiscountSum(discountSum);
                statistics.setDiscountChequeCost(discountChequeCost);
                statistics.setAverageDiscountChequeCost(discountChequeCost / saleSize);
            }
            else {
                statistics.setChequeCount(0);
                statistics.setChequeCost(0f);
                statistics.setAverageChequeCost(0f);
                statistics.setDiscountSum(0f);
                statistics.setDiscountChequeCost(0f);
                statistics.setAverageDiscountChequeCost(0f);
            }
            statisticsService.add(statistics);
        }
    }

    @RequestMapping(method = RequestMethod.GET)
    public ModelAndView getAllStatistics() {
        return getAllDomains("allStatisticsView", "allStatistics", statisticsService);
    }

}
