package app.models.classes.BeginDateEndDates;

import javax.persistence.*;
import java.util.Date;

/**
 * Created by NeiD on 23.11.2016.
 */
@Entity
@IdClass(BeginDateEndDateModelPrimaryKey.class)
@Inheritance(strategy = InheritanceType.TABLE_PER_CLASS)
public abstract class BaseBeginDateEndDateDomain {

    @Id
    @AttributeOverrides({@AttributeOverride(name = "beginDate", column = @Column(name = BeginDateEndDateModelPrimaryKey.beginDateColumnName)),
                         @AttributeOverride(name = "endDate", column = @Column(name = BeginDateEndDateModelPrimaryKey.endDateColumnName))})
    private Date beginDate, endDate;

    public BaseBeginDateEndDateDomain() {}

    protected BaseBeginDateEndDateDomain(Date beginDate, Date endDate) {
        setBeginDate(beginDate);
        setEndDate(endDate);
    }

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
