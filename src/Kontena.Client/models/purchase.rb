require_relative '../model'

class Purchase
  include Model

  service_url "#{ ENV['PURCHASE_SVC_URL'] || 'http://localhost:9997' }/purchases"

  attr_accessor :id,
                :product_id,
                :customer_id,
                :total,
                :transaction_date

  def initialize(args = {})
    args ||= {}
    @id = args[:id]
    @product_id = args[:product_id]
    @customer_id = args[:customer_id]
    @total = args[:total]
    @transaction_date = args[:transaction_date]
  end

  def save
    raise "update not supported"
  end

  def delete
    raise "delete not supported"
  end
end
