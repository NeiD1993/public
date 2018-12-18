package app.models.classes.BeginDateEndDates;

import javax.persistence.Column;
import javax.persistence.Embeddable;
import java.io.Serializable;
import java.util.Date;

/**
 * Created by NeiD on 22.11.2016.
 */
@Embeddable
public class BeginDateEndDateModelPrimaryKey implements Serializable {

    public static final String beginDateColumnName = "BEGIN_DATE";

    public static final String endDateColumnName = "END_DATE";

    @Column(name = beginDateColumnName, nullable = false, updatable = false)
    private Date beginDate;

    @Column(name = endDateColumnName, nullable = false, updatable = false)
    private Date endDate;

    public Date getBeginDate() {
        return beginDate;
    }

    public void setBeginDate(Date beginDate) {
        this.beginDate = beginDate;
    }

    public Date getEndDate() {
        return endDate;
    }

    public void setEndDate(Date endDate) {
        this.endDate = endDate;
    }

}
