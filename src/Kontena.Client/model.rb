module Model
  module ClassInterface
    attr_reader :url

    def service_url(value)
      @url = value
    end

    def list
      response = HTTParty.get(url)
      check_response! response

      JSON.parse(response.body).map { |result|
        hash = underscore_hash(result)
        self.new(hash)
      }
    end

    def retrieve(id)
      response = HTTParty.get("#{url}/#{id}")
      check_response! response

      result = JSON.parse(response.body)
      hash = underscore_hash(result)
      self.new(hash)
    end

    def create(attributes)
      body = camelize_hash(attributes).to_json
      headers = { 'Content-Type': 'application/json' }
      response = HTTParty.post("#{url}", body: body, headers: headers)
      check_response! response

      result = JSON.parse(response.body)
      hash = underscore_hash(result)
      self.new(hash)
    end

    def check_response!(response)
      unless response.code < 400
        err = "#{response.code} - #{response.message}"
        begin
          JSON.parse(response.body).each do |k, v|
            err << " - #{v.is_a?(Array) ? v.last : v}"
          end
        rescue
        end

        raise err
      end
    end

    def underscore_hash(hash)
      hash.inject({}) { |result, kv|
        key = kv[0].to_s.underscore.to_sym
        value = kv[1]
        result[key] = value
        result
      }
    end

    def camelize_hash(hash)
      hash.inject({}) { |result, kv|
        key = kv[0].to_s.camelize(:lower).to_sym
        value = kv[1]
        result[key] = value
        result
      }
    end
  end

  def attributes
    instance_variables.inject({}){ |results, ivar|
      results[ivar.to_s.sub('@', '').to_sym] = instance_variable_get ivar
      results
    }
  end

  def save
    body = self.class.camelize_hash(attributes).to_json
    headers = { 'Content-Type': 'application/json' }
    response = HTTParty.put("#{self.class.url}/#{self.id}", body: body, headers: headers)
    self.class.check_response! response
  end

  def delete
    response = HTTParty.delete("#{self.class.url}/#{self.id}")
    self.class.check_response! response
  end

  def self.included(base)
    base.extend(ClassInterface)
  end
end