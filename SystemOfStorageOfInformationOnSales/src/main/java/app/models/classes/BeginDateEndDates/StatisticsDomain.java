package app.models.classes.BeginDateEndDates;

import app.models.IDomain;

import javax.persistence.*;
import java.util.Date;

/**
 * Created by NeiD on 22.11.2016.
 */
@Entity
@Table(name = "STATISTICS")
public class StatisticsDomain extends BaseBeginDateEndDateDomain implements IDomain {

    @Column(name = "CHEQUE_COUNT", nullable = false, updatable = false)
    private Integer chequeCount;

    @Column(name = "CHEQUE_COST", nullable = false, updatable = false)
    private Float chequeCost;

    @Column(name = "AVERAGE_CHEQUE_COST", nullable = false, updatable = false)
    private Float averageChequeCost;

    @Column(name = "DISCOUNT_SUM", nullable = false, updatable = false)
    private Float discountSum;

    @Column(name = "DISCOUNT_CHEQUE_COST", nullable = false, updatable = false)
    private Float discountChequeCost;

    @Column(name = "AVERAGE_DISCOUNT_CHEQUE_COST", nullable = false, updatable = false)
    private Float averageDiscountChequeCost;

    public StatisticsDomain() {}

    public StatisticsDomain(Date beginDate, Date endDate) {
        setBeginDate(beginDate);
        setEndDate(endDate);
    }

    public Integer getChequeCount() {
        return chequeCount;
    }

    public void setChequeCount(Integer chequeCount) {
        this.chequeCount = chequeCount;
    }

    public Float getChequeCost() {
        return chequeCost;
    }

    public void setChequeCost(Float chequeCost) {
        this.chequeCost = chequeCost;
    }

    public Float getAverageChequeCost() {
        return averageChequeCost;
    }

    public void setAverageChequeCost(Float averageChequeCost) {
        this.averageChequeCost = averageChequeCost;
    }

    public Float getDiscountSum() {
        return discountSum;
    }

    public void setDiscountSum(Float discountSum) {
        this.discountSum = discountSum;
    }

    public Float getDiscountChequeCost() {
        return discountChequeCost;
    }

    public void setDiscountChequeCost(Float discountChequeCost) {
        this.discountChequeCost = discountChequeCost;
    }

    public Float getAverageDiscountChequeCost() {
        return averageDiscountChequeCost;
    }

    public void setAverageDiscountChequeCost(Float averageDiscountChequeCost) {
        this.averageDiscountChequeCost = averageDiscountChequeCost;
    }

}
