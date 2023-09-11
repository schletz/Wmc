import axios from "axios";

class CustomerService {
    constructor() {

    }
    async getCustomers() {
        const data = (await axios.get('customers')).data;
        return data;
    }
}

export default new CustomerService();
