import { Item } from "../../app/models/basket";
import { useFetchBasketQuery, useClearBasketMutation } from "../../features/basket/basketApi";

export const useBasket = () => {
    const { data: basket } = useFetchBasketQuery();
    const [clearBasket] = useClearBasketMutation();

    const subTotal = basket?.items.reduce((sum: number, item: Item) => sum + item.quantity * item.price, 0) ?? 0;
    const deliveryFee = subTotal > 10000 ? 0 : 500;

    let discount = 0;
    
    if (basket?.coupon) {
        if (basket.coupon.amountOff) {
            discount = basket.coupon.amountOff
        } else if (basket.coupon.percentOff) {
            discount = Math.round((subTotal *
                (basket.coupon.percentOff / 100)) * 100) / 100;
        }
    }

    const total = Math.round(
        (subTotal - discount + deliveryFee) * 100
    ) / 100;
    
    return { basket, subTotal, deliveryFee, discount, total, clearBasket }
}