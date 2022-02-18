<?php
class StoresController extends Controller
{
    private array $stores;

    public function get()
    {
        $this->viewData['stores'] = $this->stores;
    }

    public function getAllStores()
    {
        return $this->ok($this->stores);
    }

    public function post()
    {
    }

    // Wird vor jeder Action Methode ausgeführt.
    public function onExecute()
    {
        $this->stores = [
            ['id' => 1, 'name' => 'Store1'],
            ['id' => 2, 'name' => 'Store2'],
            ['id' => 3, 'name' => 'Store3']
        ];
    }
}
