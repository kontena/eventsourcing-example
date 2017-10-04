require_relative '../model'

class Product
  include Model

  service_url "#{ ENV['PRODUCT_SVC_URL'] || 'http://localhost:9995' }/products"

  attr_accessor :id,
                :name,
                :price

  def initialize(args = {})
    args ||= {}
    @id = args[:id]
    @name = args[:name]
    @price = args[:price]
  end
end