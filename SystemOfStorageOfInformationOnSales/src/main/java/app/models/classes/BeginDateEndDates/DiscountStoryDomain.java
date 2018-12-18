package app.models.classes.BeginDateEndDates;

import app.models.classes.productsSales.ProductDomain;
import app.models.IDomain;

import javax.persistence.*;
import javax.persistence.Entity;
import javax.persistence.Table;
import java.util.Date;

/**
 * Created by NeiD on 22.11.2016.
 */
@Entity
@Table(name = "DISCOUNT_STORY")
public class DiscountStoryDomain extends BaseBeginDateEndDateDomain implements IDomain {

    @ManyToOne
    @JoinColumn(name = "PRODUCT_NAME", nullable = false, updatable = false)
    ProductDomain product;

    @Column(name = "DISCOUNT", nullable = false, updatable = false)
    private Byte discount;

    public DiscountStoryDomain() {}

    public DiscountStoryDomain(Date beginDate, Date endDate, ProductDomain product, Byte discount) {
        super(beginDate, endDate);
        setProduct(product);
        setDiscount(discount);
    }

    public ProductDomain getProduct() {
        return product;
    }

    public void setProduct(ProductDomain product) {
        this.product = product;
    }

    public Byte getDiscount() {
        return discount;
    }

    public void setDiscount(Byte discount) {
        this.discount = discount;
    }

}
