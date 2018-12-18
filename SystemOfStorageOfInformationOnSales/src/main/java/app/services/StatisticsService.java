package app.services;

import app.models.classes.BeginDateEndDates.BeginDateEndDateModelPrimaryKey;
import app.models.classes.BeginDateEndDates.StatisticsDomain;
import app.services.base.AddedService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

/**
 * Created by NeiD on 25.11.2016.
 */
@Service("statisticsService")
@Transactional
public class StatisticsService extends AddedService<BeginDateEndDateModelPrimaryKey, StatisticsDomain> {

    protected String getClassName() {
        return StatisticsDomain.class.getSimpleName();
    }

}
