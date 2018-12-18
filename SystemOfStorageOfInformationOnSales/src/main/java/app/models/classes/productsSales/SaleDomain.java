package app.models.classes.productsSales;

import app.models.IDomain;
import org.hibernate.annotations.*;
import org.hibernate.annotations.CascadeType;

import javax.persistence.*;
import javax.persistence.Entity;
import javax.persistence.Table;
import java.util.Date;
import java.util.List;

/**
 * Created by NeiD on 21.11.2016.
 */
@Entity
@Table(name = "SALES")
public class SaleDomain implements IDomain {

    @Id
    @Column(name = "ID", updatable = false)
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @Column(name = "DATE", nullable = false, updatable = false)
    private Date date;

    @OneToMany(mappedBy = "sale")
    @Cascade(CascadeType.SAVE_UPDATE)
    private List<ProductSaleDomain> products;

    @Column(name = "COST", nullable = false, updatable = false)
    private Float cost;

    @Column(name = "DISCOUNT", nullable = false, updatable = false)
    private Float discount;

    public Integer getId() {
        return id;
    }

    public Date getDate() {
        return date;
    }

    public void setDate(Date date) {
        this.date = date;
    }

    public List<ProductSaleDomain> getProducts() {
        return products;
    }

    public void setProducts(List<ProductSaleDomain> products) {
        this.products = products;
    }

    public Float getCost() {
        return cost;
    }

    public void setCost(Float cost) {
        this.cost = cost;
    }

    public Float getDiscount() {
        return discount;
    }

    public void setDiscount(Float discount) {
        this.discount = discount;
    }

}
